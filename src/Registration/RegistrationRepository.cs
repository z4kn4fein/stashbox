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

namespace Stashbox.Registration
{
    internal class RegistrationRepository : IRegistrationRepository
    {
        private ImmutableTree<Type, ImmutableBucket<object, ServiceRegistration>> serviceRepository = ImmutableTree<Type, ImmutableBucket<object, ServiceRegistration>>.Empty;
        private readonly ContainerConfiguration containerConfiguration;

        private readonly IRegistrationSelectionRule[] filters =
        {
            RegistrationSelectionRules.GenericFilter,
            RegistrationSelectionRules.NameFilter,
            RegistrationSelectionRules.ScopeNameFilter,
            RegistrationSelectionRules.ConditionFilter
        };

        private readonly IRegistrationSelectionRule[] topLevelFilters =
        {
            RegistrationSelectionRules.GenericFilter,
            RegistrationSelectionRules.NameFilter,
            RegistrationSelectionRules.ScopeNameFilter
        };

        private readonly IRegistrationSelectionRule[] enumerableFilters =
        {
            RegistrationSelectionRules.GenericFilter,
            RegistrationSelectionRules.ScopeNameFilter,
            RegistrationSelectionRules.ConditionFilter
        };

        public RegistrationRepository(ContainerConfiguration containerConfiguration)
        {
            this.containerConfiguration = containerConfiguration;
        }

        public bool AddOrUpdateRegistration(ServiceRegistration registration, Type serviceType)
        {
            if (registration.RegistrationContext.ReplaceExistingRegistrationOnlyIfExists)
                return Swap.SwapValue(ref this.serviceRepository, (reg, type, t3, t4, repo) =>
                    repo.UpdateIfExists(type, true, regs => regs.ReplaceIfExists(reg.RegistrationDiscriminator, reg, false,
                        (old, @new) =>
                        {
                            @new.Replaces(old);
                            return @new;
                        })),
                    registration,
                    serviceType,
                    Constants.DelegatePlaceholder,
                    Constants.DelegatePlaceholder);

            return Swap.SwapValue(ref this.serviceRepository, (reg, type, newRepo, regBehavior, repo) =>
                repo.AddOrUpdate(type, newRepo, true,
                    (oldValue, newValue) =>
                    {
                        var allowUpdate = reg.RegistrationContext.ReplaceExistingRegistration ||
                                          regBehavior == Rules.RegistrationBehavior.ReplaceExisting;

                        if (!allowUpdate && regBehavior == Rules.RegistrationBehavior.PreserveDuplications)
                            return oldValue.Add(reg.RegistrationDiscriminator, reg);

                        return oldValue.AddOrUpdate(reg.RegistrationDiscriminator, reg, false,
                            (old, @new) =>
                            {
                                if (!allowUpdate && regBehavior == Rules.RegistrationBehavior.ThrowException)
                                    throw new ServiceAlreadyRegisteredException(old.ImplementationType);

                                if (!allowUpdate)
                                    return old;

                                @new.Replaces(old);
                                return @new;
                            });
                    }),
                    registration,
                    serviceType,
                    new ImmutableBucket<object, ServiceRegistration>(registration.RegistrationDiscriminator, registration),
                    this.containerConfiguration.RegistrationBehavior);
        }

        public bool AddOrReMapRegistration(ServiceRegistration registration, Type serviceType) =>
            registration.RegistrationContext.ReplaceExistingRegistrationOnlyIfExists
                ? Swap.SwapValue(ref this.serviceRepository, (type, newRepo, t3, t4, repo) =>
                    repo.UpdateIfExists(type, newRepo, true), serviceType,
                    new ImmutableBucket<object, ServiceRegistration>(registration.RegistrationDiscriminator, registration),
                    Constants.DelegatePlaceholder,
                    Constants.DelegatePlaceholder)
                : Swap.SwapValue(ref this.serviceRepository, (type, newRepo, t3, t4, repo) =>
                    repo.AddOrUpdate(type, newRepo, true, true), serviceType,
                    new ImmutableBucket<object, ServiceRegistration>(registration.RegistrationDiscriminator, registration),
                    Constants.DelegatePlaceholder,
                    Constants.DelegatePlaceholder);

        public bool ContainsRegistration(Type type, object name, bool includeOpenGenerics = false) =>
            serviceRepository.ContainsRegistration(type, name, includeOpenGenerics);

        public IEnumerable<KeyValuePair<Type, ServiceRegistration>> GetRegistrationMappings() =>
             serviceRepository.Walk().SelectMany(reg => reg.Value.Select(r => new KeyValuePair<Type, ServiceRegistration>(reg.Key, r)));

        public ServiceRegistration GetRegistrationOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            this.GetRegistrationsForType(typeInfo.Type)?.SelectOrDefault(typeInfo, resolutionContext, 
                resolutionContext.IsTopRequest ? this.topLevelFilters : this.filters);

        public IEnumerable<ServiceRegistration> GetRegistrationsOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            this.GetRegistrationsForType(typeInfo.Type)
                ?.FilterExclusiveOrDefault(typeInfo, resolutionContext, this.enumerableFilters)
                ?.OrderBy(reg => reg.RegistrationOrder);

        private IEnumerable<ServiceRegistration> GetRegistrationsForType(Type type)
        {
            IEnumerable<ServiceRegistration> registrations = serviceRepository.GetOrDefaultByRef(type);
            if (!type.IsClosedGenericType()) return registrations;

            var openGenerics = serviceRepository.GetOrDefaultByRef(type.GetGenericTypeDefinition());

            if (openGenerics != null)
                registrations = registrations == null ? openGenerics : openGenerics.Concat(registrations);

            var variantGenerics = serviceRepository.Walk()
                .Where(r => r.Key.IsGenericType &&
                    r.Key.GetGenericTypeDefinition() == type.GetGenericTypeDefinition() &&
                    r.Key != type &&
                    r.Key.ImplementsWithoutGenericCheck(type))
                .SelectMany(r => r.Value).ToArray();

            if (variantGenerics.Length > 0)
                registrations = registrations == null ? variantGenerics : variantGenerics.Concat(registrations);

            return registrations;
        }
    }
}
