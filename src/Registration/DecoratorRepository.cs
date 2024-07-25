﻿using Stashbox.Registration.Extensions;
using Stashbox.Registration.SelectionRules;
using Stashbox.Resolution;
using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;
using System.Linq;
using Stashbox.Configuration;

namespace Stashbox.Registration;

internal class DecoratorRepository(ContainerConfiguration containerConfiguration) : IDecoratorRepository
{
    private ImmutableTree<Type, ImmutableBucket<Type, ServiceRegistration>> repository = ImmutableTree<Type, ImmutableBucket<Type, ServiceRegistration>>.Empty;

    private readonly IRegistrationSelectionRule[] filters =
    [
        RegistrationSelectionRules.GenericFilter,
        RegistrationSelectionRules.ConditionFilter,
        RegistrationSelectionRules.ScopeNameFilter,
        RegistrationSelectionRules.DecoratorFilter
    ];

    public void AddDecorator(Type type, ServiceRegistration serviceRegistration, bool remap)
    {
        var newRepository = ImmutableBucket<Type, ServiceRegistration>.Empty.Add(serviceRegistration.ImplementationType, serviceRegistration);

        if (remap)
            Swap.SwapValue(ref this.repository, (t1, t2, _, _, repo) =>
                    repo.AddOrUpdate(t1, t2, true, (_, newValue) => newValue), type, newRepository,
                Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
        else
            Swap.SwapValue(ref this.repository, (t1, t2, t3, _, repo) =>
                    repo.AddOrUpdate(t1, t2, true, (oldValue, _) => oldValue
                        .AddOrUpdate(t3.ImplementationType, t3, t3.Options.IsOn(RegistrationOption.ReplaceExistingRegistration))),
                type, newRepository, serviceRegistration, Constants.DelegatePlaceholder);
    }

    public IEnumerable<ServiceRegistration>? GetDecoratorsOrDefault(Type implementationTypeToDecorate, TypeInformation typeInformation, ResolutionContext resolutionContext) =>
        this.GetRegistrationsForType(typeInformation.Type)?.FilterInclusive(typeInformation.Clone(implementationTypeToDecorate), resolutionContext, this.filters);

    public IEnumerable<KeyValuePair<Type, ServiceRegistration>> GetRegistrationMappings() =>
        repository.Walk().SelectMany(reg => reg.Value.Select(r => new KeyValuePair<Type, ServiceRegistration>(reg.Key, r)));

    private IEnumerable<ServiceRegistration>? GetRegistrationsForType(Type type)
    {
        IEnumerable<ServiceRegistration>? registrations = repository.GetOrDefaultByRef(type);
        if (!type.IsClosedGenericType()) return registrations;
        
        var openGenerics = repository.GetOrDefaultByRef(type.GetGenericTypeDefinition());

        if (openGenerics != null)
            registrations = registrations == null ? openGenerics : openGenerics.Concat(registrations);

        if (!containerConfiguration.VariantGenericTypesEnabled)
            return registrations;
        
        var variantGenerics = repository.Walk()
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