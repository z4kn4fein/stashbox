using Stashbox.Entity;
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
        public static bool ContainsRegistration(this ImmutableTree<Type, ImmutableArray<object, IServiceRegistration>> repository, Type type, object name)
        {
            var registrations = repository.GetOrDefault(type);
            if (name != null && registrations != null)
                return registrations.GetOrDefault(name) != null;

            if (registrations != null || !type.IsClosedGenericType()) return registrations != null;

            registrations = repository.GetOrDefault(type.GetGenericTypeDefinition());
            return registrations?.Any(reg => type.SatisfiesGenericConstraintsOf(reg.ImplementationTypeInfo)) ?? false;
        }

        public static IServiceRegistration SelectOrDefault(this IEnumerable<IServiceRegistration> registrations,
            TypeInformation typeInformation,
            ResolutionContext resolutionContext,
            IRegistrationSelectionRule[] registrationSelectionRules)
        {
            var maxIndex = 0;
            var maxWeight = 0;
            var result = new List<IServiceRegistration>();

            foreach (var serviceRegistration in registrations)
            {
                if (!registrationSelectionRules.IsSelectionPassed(typeInformation, serviceRegistration, resolutionContext, out var weight))
                    continue;

                result.Add(serviceRegistration);

                if (weight < maxWeight) continue;
                maxWeight = weight;
                maxIndex = result.Count - 1;
            }

            return result.Count == 0 ? null : result[maxIndex];
        }

        public static IEnumerable<IServiceRegistration> FilterOrDefault(this IEnumerable<IServiceRegistration> registrations,
            TypeInformation typeInformation,
            ResolutionContext resolutionContext,
            IRegistrationSelectionRule[] registrationSelectionRules)
        {
            var common = new List<IServiceRegistration>();
            var priority = new List<IServiceRegistration>();

            foreach (var serviceRegistration in registrations)
            {
                if (!registrationSelectionRules.IsSelectionPassed(typeInformation, serviceRegistration, resolutionContext, out var weight))
                    continue;

                if (weight > 0)
                    priority.Add(serviceRegistration);

                common.Add(serviceRegistration);
            }

            return common.Count == 0
                ? null
                : priority.Count > 0
                    ? priority
                    : common;
        }

        private static bool IsSelectionPassed(this IRegistrationSelectionRule[] registrationSelectionRules,
            TypeInformation typeInformation, IServiceRegistration serviceRegistration,
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
