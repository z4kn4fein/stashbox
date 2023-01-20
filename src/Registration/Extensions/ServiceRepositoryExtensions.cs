using Stashbox.Registration.SelectionRules;
using Stashbox.Resolution;
using Stashbox.Utils.Data;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.Registration.Extensions;

internal static class ServiceRepositoryExtensions
{
    public static bool ContainsRegistration(this ImmutableTree<Type, ImmutableBucket<ServiceRegistration>> repository, Type type, object? name, bool includeOpenGenerics)
    {
        var registrations = repository.GetOrDefaultByRef(type);
        if (name != null && registrations != null)
            return registrations.Repository.Any(reg => name.Equals(reg.Name));

        if (registrations != null || !includeOpenGenerics || !type.IsClosedGenericType()) return registrations != null;

        registrations = repository.GetOrDefaultByRef(type.GetGenericTypeDefinition());
        return registrations?.Repository.Any(reg => reg.ImplementationType.SatisfiesGenericConstraintsOf(type)) ?? false;
    }

    public static ServiceRegistration? SelectOrDefault(this IEnumerable<ServiceRegistration> registrations,
        TypeInformation typeInformation,
        ResolutionContext resolutionContext,
        IRegistrationSelectionRule[] registrationSelectionRules)
    {
        var maxWeight = 0;
        ServiceRegistration? result = null;

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

    public static IEnumerable<ServiceRegistration>? FilterExclusiveOrDefault(this IEnumerable<ServiceRegistration> registrations,
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
            else
                common.Add(serviceRegistration);
        }

        if (common.Length == 0 && priority.Length == 0)
            return null;

        return priority.Length > 0
            ? priority
            : common;
    }

    public static IEnumerable<ServiceRegistration> FilterInclusive(this IEnumerable<ServiceRegistration> registrations,
        TypeInformation typeInformation,
        ResolutionContext resolutionContext,
        IRegistrationSelectionRule[] registrationSelectionRules) =>
        registrations.Where(serviceRegistration => registrationSelectionRules.IsSelectionPassed(typeInformation, serviceRegistration, resolutionContext, out _));

    private static bool IsSelectionPassed(this IRegistrationSelectionRule[] registrationSelectionRules,
        TypeInformation typeInformation, ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, out int weight)
    {
        weight = 0;
        var filterLength = registrationSelectionRules.Length;
        for (var i = 0; i < filterLength; i++)
        {
            if (!registrationSelectionRules[i].IsValidForCurrentRequest(typeInformation, serviceRegistration, resolutionContext, out var shouldIncrementWeight))
                return false;

            if (shouldIncrementWeight)
                weight++;
        }

        return true;
    }
}