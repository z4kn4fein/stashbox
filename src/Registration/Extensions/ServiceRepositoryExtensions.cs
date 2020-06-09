using Stashbox.Registration.SelectionRules;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.Registration.Extensions
{
    internal static class ServiceRepositoryExtensions
    {
        public static bool ContainsRegistration(this ImmutableTree<Type, ImmutableArray<object, ServiceRegistration>> repository, Type type, object name)
        {
            var registrations = repository.GetOrDefault(type);
            if (name != null && registrations != null)
                return registrations.GetOrDefault(name, false) != null;

            if (registrations != null || !type.IsClosedGenericType()) return registrations != null;

            registrations = repository.GetOrDefault(type.GetGenericTypeDefinition());
            return registrations?.Any(reg => type.SatisfiesGenericConstraintsOf(reg.ImplementationTypeInfo)) ?? false;
        }

        public static ServiceRegistration SelectOrDefault(this IEnumerable<ServiceRegistration> registrations,
            TypeInformation typeInformation,
            ResolutionContext resolutionContext,
            IRegistrationSelectionRule[] registrationSelectionRules)
        {
            var maxWeight = 0;
            ServiceRegistration result = null;
            foreach (var serviceRegistration in registrations)
            {
                if (!registrationSelectionRules.IsSelectionPassed(typeInformation, serviceRegistration, resolutionContext, out var weight))
                    continue;

                if (weight < maxWeight) continue;
                maxWeight = weight;
                result = serviceRegistration;
            }

            return result;
        }

        public static IEnumerable<ServiceRegistration> FilterOrDefault(this IEnumerable<ServiceRegistration> registrations,
            TypeInformation typeInformation,
            ResolutionContext resolutionContext,
            IRegistrationSelectionRule[] registrationSelectionRules)
        {
            var common = new ExpandableArray<ServiceRegistration>();
            var priority = new ExpandableArray<ServiceRegistration>();

            foreach (var serviceRegistration in registrations)
            {
                if (!registrationSelectionRules.IsSelectionPassed(typeInformation, serviceRegistration, resolutionContext, out var weight))
                    continue;

                if (weight > 0)
                    priority.Add(serviceRegistration);

                common.Add(serviceRegistration);
            }

            if (common.Length == 0)
                return null;

            return priority.Length > 0
                    ? priority
                    : common;
        }

        private static bool IsSelectionPassed(this IRegistrationSelectionRule[] registrationSelectionRules,
            TypeInformation typeInformation, ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, out int weight)
        {
            weight = 0;
            var filterLength = registrationSelectionRules.Length;
            for (var i = 0; i < filterLength; i++)
            {
                if (!registrationSelectionRules[i].IsValidForCurrentRequest(typeInformation, serviceRegistration, resolutionContext))
                    return false;

                if (registrationSelectionRules[i].ShouldIncrementWeight(typeInformation, serviceRegistration, resolutionContext))
                    weight++;
            }

            return true;
        }
    }
}
