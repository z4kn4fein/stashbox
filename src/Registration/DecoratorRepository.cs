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
    internal class DecoratorRepository : IDecoratorRepository
    {
        private ImmutableTree<Type, ImmutableBucket<Type, ServiceRegistration>> repository = ImmutableTree<Type, ImmutableBucket<Type, ServiceRegistration>>.Empty;

        private readonly IRegistrationSelectionRule[] filters =
        {
            RegistrationSelectionRules.GenericFilter,
            RegistrationSelectionRules.ConditionFilter,
            RegistrationSelectionRules.ScopeNameFilter,
            RegistrationSelectionRules.DecoratorFilter,
        };

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
            var registrations = repository.GetOrDefaultByRef(type);
            return !type.IsClosedGenericType()
                ? registrations
                : repository.GetOrDefaultByRef(type.GetGenericTypeDefinition());
        }

    }
}
