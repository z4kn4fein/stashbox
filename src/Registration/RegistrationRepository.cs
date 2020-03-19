using Stashbox.Entity;
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
        private ImmutableTree<Type, ImmutableArray<object, IServiceRegistration>> serviceRepository = ImmutableTree<Type, ImmutableArray<object, IServiceRegistration>>.Empty;

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
            RegistrationSelectionRules .GenericFilter,
            RegistrationSelectionRules .ScopeNameFilter,
            RegistrationSelectionRules .ConditionFilter
        };
        
        public void AddOrUpdateRegistration(IServiceRegistration registration, Type serviceType, bool remap, bool replace)
        {
            var newRepository = new ImmutableArray<object, IServiceRegistration>(registration.RegistrationId, registration);

            if (remap)
                Swap.SwapValue(ref serviceRepository, (t1, t2, t3, t4, repo) =>
                    repo.AddOrUpdate(t1, t2, true), serviceType, newRepository, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            else
                Swap.SwapValue(ref serviceRepository, (t1, t2, t3, t4, repo) =>
                    repo.AddOrUpdate(t2, t3,
                        (oldValue, newValue) => oldValue.AddOrUpdate(t1.RegistrationId, t1, t4)),
                        registration, serviceType, newRepository, replace);
        }

        public bool ContainsRegistration(Type type, object name) =>
            serviceRepository.ContainsRegistration(type, name);

        public IEnumerable<KeyValuePair<Type, IServiceRegistration>> GetRegistrationMappings() =>
             serviceRepository.Walk().SelectMany(reg => reg.Value.Select(r => new KeyValuePair<Type, IServiceRegistration>(reg.Key, r)));

        public IServiceRegistration GetRegistrationOrDefault(Type type, ResolutionContext resolutionContext, object name = null)
        {
            var registrations = this.GetRegistrationsForType(type);
            if (registrations == null) return null;

            var filtered = registrations.SelectOrDefault(new TypeInformation { Type = type, DependencyName = name }, resolutionContext, this.topLevelFilters, out var maxIndex);
            return filtered?[maxIndex].Key;
        }

        public IServiceRegistration GetRegistrationOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var registrations = this.GetRegistrationsForType(typeInfo.Type);
            if (registrations == null) return null;

            var filtered = registrations.SelectOrDefault(typeInfo, resolutionContext, this.filters, out var maxIndex);
            return filtered?[maxIndex].Key;
        }

        public IEnumerable<IServiceRegistration> GetRegistrationsOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var registrations = this.GetRegistrationsForType(typeInfo.Type);
            return registrations?.FilterOrDefault(typeInfo, resolutionContext, this.enumerableFilters);
        }

        private IEnumerable<IServiceRegistration> GetRegistrationsForType(Type type)
        {
            var registrations = serviceRepository.GetOrDefault(type);
            if (!type.IsClosedGenericType()) return registrations;
            
            var openGenerics = serviceRepository.GetOrDefault(type.GetGenericTypeDefinition());

            if (openGenerics == null) return registrations;
            return registrations == null ? openGenerics : openGenerics.Concat(registrations);
        }
    }
}
