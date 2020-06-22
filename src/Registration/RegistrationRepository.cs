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
        private ImmutableTree<Type, IImmutableArray<object, ServiceRegistration>> serviceRepository = ImmutableTree<Type, IImmutableArray<object, ServiceRegistration>>.Empty;
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

        public void AddOrUpdateRegistration(ServiceRegistration registration, Type serviceType, bool remap, bool replace)
        {
            var newRepository = ImmutableArray<object, ServiceRegistration>.Empty.Add(registration.RegistrationDiscriminator, registration);

            if (remap)
                Swap.SwapValue(ref serviceRepository, (type, newRepo, t3, t4, repo) =>
                    repo.AddOrUpdate(type, newRepo, true), serviceType, newRepository, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            else
                Swap.SwapValue(ref serviceRepository, (reg, type, newRepo, allowUpdate, repo) =>
                    repo.AddOrUpdate(type, newRepo,
                        (oldValue, newValue) =>
                        {
                            if (!allowUpdate && this.containerConfiguration.RegistrationBehavior == Rules.RegistrationBehavior.PreserveDuplications)
                                return oldValue.Add(reg.RegistrationDiscriminator, reg);

                            return oldValue.AddOrUpdate(reg.RegistrationDiscriminator, reg, false,
                                (old, @new) =>
                                {
                                    if (!allowUpdate && this.containerConfiguration.RegistrationBehavior == Rules.RegistrationBehavior.ThrowException)
                                        throw new ServiceAlreadyRegisteredException(old.ImplementationType);

                                    if (!allowUpdate)
                                        return old;

                                    @new.Replaces(old);
                                    return @new;
                                });
                        }),
                        registration, serviceType, newRepository,
                        replace ||
                        this.containerConfiguration.RegistrationBehavior == Rules.RegistrationBehavior.ReplaceExisting);
        }

        public bool ContainsRegistration(Type type, object name) =>
            serviceRepository.ContainsRegistration(type, name);

        public IEnumerable<KeyValuePair<Type, ServiceRegistration>> GetRegistrationMappings() =>
             serviceRepository.Walk().SelectMany(reg => reg.Value.Select(r => new KeyValuePair<Type, ServiceRegistration>(reg.Key, r)));

        public ServiceRegistration GetRegistrationOrDefault(Type type, ResolutionContext resolutionContext, object name = null) =>
            this.GetRegistrationsForType(type)?.SelectOrDefault(new TypeInformation(type, name), resolutionContext, this.topLevelFilters);

        public ServiceRegistration GetRegistrationOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            this.GetRegistrationsForType(typeInfo.Type)?.SelectOrDefault(typeInfo, resolutionContext, this.filters);

        public IEnumerable<ServiceRegistration> GetRegistrationsOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            this.GetRegistrationsForType(typeInfo.Type)?.FilterOrDefault(typeInfo, resolutionContext, this.enumerableFilters)?.OrderBy(reg => reg.RegistrationId);

        private IEnumerable<ServiceRegistration> GetRegistrationsForType(Type type)
        {
            var registrations = serviceRepository.GetOrDefault(type);
            if (!type.IsClosedGenericType()) return registrations;

            var openGenerics = serviceRepository.GetOrDefault(type.GetGenericTypeDefinition());

            if (openGenerics == null) return registrations;
            return registrations == null ? openGenerics : openGenerics.Concat(registrations);
        }
    }
}
