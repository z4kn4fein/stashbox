using Stashbox.Configuration;
using Stashbox.Exceptions;
using Stashbox.Registration.Extensions;
using Stashbox.Registration.SelectionRules;
using Stashbox.Resolution;
using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.Registration;

internal class RegistrationRepository : IRegistrationRepository
{
    private ImmutableTree<Type, ImmutableBucket<ServiceRegistration>> serviceRepository = ImmutableTree<Type, ImmutableBucket<ServiceRegistration>>.Empty;
    private readonly ContainerConfiguration containerConfiguration;

    private readonly IRegistrationSelectionRule[] filters =
    {
        RegistrationSelectionRules.GenericFilter,
        RegistrationSelectionRules.NameFilter,
        RegistrationSelectionRules.MetadataFilter,
        RegistrationSelectionRules.ScopeNameFilter,
        RegistrationSelectionRules.ConditionFilter,
    };

    private readonly IRegistrationSelectionRule[] topLevelFilters =
    {
        RegistrationSelectionRules.GenericFilter,
        RegistrationSelectionRules.NameFilter,
        RegistrationSelectionRules.MetadataFilter,
        RegistrationSelectionRules.ScopeNameFilter,
    };

    private readonly IRegistrationSelectionRule[] enumerableFilters =
    {
        RegistrationSelectionRules.GenericFilter,
        RegistrationSelectionRules.EnumerableNameFilter,
        RegistrationSelectionRules.ScopeNameFilter,
        RegistrationSelectionRules.ConditionFilter,
        RegistrationSelectionRules.MetadataFilter,
    };

    public RegistrationRepository(ContainerConfiguration containerConfiguration)
    {
        this.containerConfiguration = containerConfiguration;
    }

    public bool AddOrUpdateRegistration(ServiceRegistration registration, Type serviceType)
    {
        if (registration.Options.IsOn(RegistrationOption.ReplaceExistingRegistrationOnlyIfExists))
            return Swap.SwapValue(ref this.serviceRepository, (reg, type, _, _, repo) =>
                    repo.UpdateIfExists(type, true, regs =>
                    {
                        int existingIndex = -1;
                        for (int i = 0; i < regs.Length; i++)
                        {
                            var current = regs[i];
                            var existingDiscriminator = current.Name ?? current.ImplementationType;
                            var newDiscriminator = reg.Name ?? reg.ImplementationType;
                            if (existingDiscriminator.Equals(newDiscriminator))
                                existingIndex = i;
                        }

                        if (existingIndex != -1)
                        {
                            var replaced = regs[existingIndex];
                            reg.Replaces(replaced);
                            return regs.ReplaceAt(existingIndex, reg);
                        }

                        return regs;
                    }),
                registration,
                serviceType,
                Constants.DelegatePlaceholder,
                Constants.DelegatePlaceholder);

        return Swap.SwapValue(ref this.serviceRepository, (reg, type, newRepo, regBehavior, repo) =>
                repo.AddOrUpdate(type, newRepo, true,
                    (oldValue, _) =>
                    {
                        var replaceExisting = reg.Options.IsOn(RegistrationOption.ReplaceExistingRegistration);
                        var allowUpdate = replaceExisting || regBehavior == Rules.RegistrationBehavior.ReplaceExisting;

                        if (!allowUpdate && regBehavior == Rules.RegistrationBehavior.PreserveDuplications)
                            return oldValue.Add(reg);

                        int existingIndex = -1;
                        for (int i = 0; i < oldValue.Length; i++)
                        {
                            var current = oldValue[i];
                            if (!replaceExisting && current.ImplementationType != reg.ImplementationType) continue;
                            var existingDiscriminator = current.Name ?? current.ImplementationType;
                            var newDiscriminator = reg.Name ?? reg.ImplementationType;
                            if (existingDiscriminator.Equals(newDiscriminator))
                                existingIndex = i;
                        }

                        if (existingIndex != -1)
                        {
                            switch (allowUpdate)
                            {
                                case false when regBehavior == Rules.RegistrationBehavior.ThrowException:
                                    throw new ServiceAlreadyRegisteredException(reg.ImplementationType);
                                case false:
                                    return oldValue;
                                default:
                                    var replaced = oldValue[existingIndex];
                                    reg.Replaces(replaced);
                                    return oldValue.ReplaceAt(existingIndex, reg);
                            }
                        }

                        return oldValue.Add(reg);
                    }),
            registration,
            serviceType,
            new ImmutableBucket<ServiceRegistration>(registration),
            this.containerConfiguration.RegistrationBehavior);
    }

    public bool AddOrReMapRegistration(ServiceRegistration registration, Type serviceType) =>
        registration.Options.IsOn(RegistrationOption.ReplaceExistingRegistrationOnlyIfExists)
            ? Swap.SwapValue(ref this.serviceRepository, (type, newRepo, _, _, repo) =>
                    repo.UpdateIfExists(type, newRepo, true), serviceType,
                new ImmutableBucket<ServiceRegistration>(registration),
                Constants.DelegatePlaceholder,
                Constants.DelegatePlaceholder)
            : Swap.SwapValue(ref this.serviceRepository, (type, newRepo, _, _, repo) =>
                    repo.AddOrUpdate(type, newRepo, true, true), serviceType,
                new ImmutableBucket<ServiceRegistration>(registration),
                Constants.DelegatePlaceholder,
                Constants.DelegatePlaceholder);

    public bool ContainsRegistration(Type type, object? name, bool includeOpenGenerics = true) =>
        serviceRepository.ContainsRegistration(type, name, includeOpenGenerics);

    public IEnumerable<KeyValuePair<Type, ServiceRegistration>> GetRegistrationMappings() =>
        serviceRepository.Walk().SelectMany(reg => reg.Value.Repository.Select(r => new KeyValuePair<Type, ServiceRegistration>(reg.Key, r)));

    public ServiceRegistration? GetRegistrationOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
        this.GetRegistrationsForType(typeInfo.Type)?.SelectOrDefault(typeInfo, resolutionContext,
            resolutionContext.IsTopRequest ? this.topLevelFilters : this.filters);

    public IEnumerable<ServiceRegistration>? GetRegistrationsOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
        this.GetRegistrationsForType(typeInfo.Type)
            ?.FilterExclusiveOrDefault(typeInfo, resolutionContext, this.enumerableFilters)
            ?.OrderBy(reg => reg.RegistrationOrder);

    private IEnumerable<ServiceRegistration>? GetRegistrationsForType(Type type)
    {
        IEnumerable<ServiceRegistration>? registrations = serviceRepository.GetOrDefaultByRef(type)?.Repository;
        if (!type.IsClosedGenericType()) return registrations;

        var openGenerics = serviceRepository.GetOrDefaultByRef(type.GetGenericTypeDefinition())?.Repository;

        if (openGenerics != null)
            registrations = registrations == null ? openGenerics : openGenerics.Concat(registrations);

        var variantGenerics = serviceRepository.Walk()
            .Where(r => r.Key.IsGenericType &&
                        r.Key.GetGenericTypeDefinition() == type.GetGenericTypeDefinition() &&
                        r.Key != type &&
                        r.Key.ImplementsWithoutGenericCheck(type))
            .SelectMany(r => r.Value.Repository).ToArray();

        if (variantGenerics.Length > 0)
            registrations = registrations == null ? variantGenerics : variantGenerics.Concat(registrations);

        return registrations;
    }
}