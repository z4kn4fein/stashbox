using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Stashbox.Entity;
using Stashbox.Registration.Filters;
using Stashbox.Resolution;

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
            return registrations?.Any(reg => reg.ValidateGenericConstraints(type)) ?? false;
        }

        public static ArrayList<KeyValuePair<IServiceRegistration, int>> FilterOrDefault(this IEnumerable<IServiceRegistration> registrations,
            TypeInformation typeInformation,
            ResolutionContext resolutionContext,
            IRegistrationFilter[] registrationFilters,
            out int maxIndex)
        {
            maxIndex = 0;
            var result = new ArrayList<KeyValuePair<IServiceRegistration, int>>();
            var maxWeight = 0;

            foreach (var serviceRegistration in registrations)
            {
                if (!registrationFilters.AreFiltersPassed(typeInformation, serviceRegistration, resolutionContext, out var weight))
                    continue;

                result.Add(new KeyValuePair<IServiceRegistration, int>(serviceRegistration, weight));

                if (weight < maxWeight) continue;
                maxWeight = weight;
                maxIndex = result.Length - 1;
            }

            return result.Length == 0 ? null : result;
        }

        public static IEnumerable<IServiceRegistration> FilterOrDefault(this IEnumerable<IServiceRegistration> registrations,
            TypeInformation typeInformation,
            ResolutionContext resolutionContext,
            IRegistrationFilter[] registrationFilters)
        {
            var common = new ArrayList<IServiceRegistration>();
            var priority = new ArrayList<IServiceRegistration>();

            foreach (var serviceRegistration in registrations)
            {
                if (!registrationFilters.AreFiltersPassed(typeInformation, serviceRegistration, resolutionContext, out var weight))
                    continue;

                if (weight > 0)
                    priority.Add(serviceRegistration);

                common.Add(serviceRegistration);
            }

            return common.Length == 0 
                ? null 
                : priority.Length > 0 
                    ? priority 
                    : common;
        }

        private static bool AreFiltersPassed(this IRegistrationFilter[] registrationFilters,
            TypeInformation typeInformation, IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, out int weight)
        {
            weight = 0;
            var filterLength = registrationFilters.Length;
            for (var i = 0; i < filterLength; i++)
            {
                if (!registrationFilters[i].IsValidForCurrentRequest(typeInformation, serviceRegistration, resolutionContext))
                    return false;

                if (registrationFilters[i].ShouldIncrementWeight(typeInformation, serviceRegistration, resolutionContext))
                    weight++;
            }

            return true;
        }
    }
}
