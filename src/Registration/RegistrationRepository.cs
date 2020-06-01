using Stashbox.Configuration;
using Stashbox.Registration.Extensions;
using Stashbox.Registration.SelectionRules;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.Registration
{
    internal class RegistrationRepository : IRegistrationRepository
    {
        private ImmutableTree<Type, ImmutableArray<object, ServiceRegistration>> serviceRepository = ImmutableTree<Type, ImmutableArray<object, ServiceRegistration>>.Empty;
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
            var newRepository = new ImmutableArray<object, ServiceRegistration>(registration.RegistrationName, registration);

            if (remap)
                Swap.SwapValue(ref serviceRepository, (t1, t2, t3, t4, repo) =>
                    repo.AddOrUpdate(t1, t2, true), serviceType, newRepository, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            else
                Swap.SwapValue(ref serviceRepository, (t1, t2, t3, t4, repo) =>
                    repo.AddOrUpdate(t2, t3,
                        (oldValue, newValue) =>
                            oldValue.AddOrUpdate(t1.RegistrationName, t1, false, t4, (old, @new) => @new.Replaces(old))),
                        registration, serviceType, newRepository,
                        replace ||
                        this.containerConfiguration.RegistrationBehavior == Rules.RegistrationBehavior.ReplaceExisting ||
                        this.containerConfiguration.RegistrationBehavior == Rules.RegistrationBehavior.ThrowException);
        }

        public bool ContainsRegistration(Type type, object name) =>
            serviceRepository.ContainsRegistration(type, name);

        public IEnumerable<KeyValuePair<Type, ServiceRegistration>> GetRegistrationMappings() =>
             serviceRepository.Walk().SelectMany(reg => reg.Value.Select(r => new KeyValuePair<Type, ServiceRegistration>(reg.Key, r)));

        public ServiceRegistration GetRegistrationOrDefault(Type type, ResolutionContext resolutionContext, object name = null) =>
            this.GetRegistrationsForType(type)?.SelectOrDefault(new TypeInformation { Type = type, DependencyName = name }, resolutionContext, this.topLevelFilters);

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
