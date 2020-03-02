using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Registration.Extensions;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.Registration
{
    internal class RegistrationRepository2 : IRegistrationRepository
    {
        private readonly ContainerConfiguration containerConfiguration;
        private ImmutableTree<Type, ImmutableArray<object, IServiceRegistration>> serviceRepository = ImmutableTree<Type, ImmutableArray<object, IServiceRegistration>>.Empty;

        public RegistrationRepository2(ContainerConfiguration containerConfiguration)
        {
            this.containerConfiguration = containerConfiguration;
        }

        public void AddOrUpdateRegistration(IServiceRegistration registration, Type serviceType, bool remap, bool replace)
        {
            var newRepository = new ImmutableArray<object, IServiceRegistration>(registration.RegistrationId, registration);

            if (remap)
                Swap.SwapValue(ref this.serviceRepository, (t1, t2, t3, t4, repo) =>
                    repo.AddOrUpdate(t1, t2, true), serviceType, newRepository, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            else
                Swap.SwapValue(ref this.serviceRepository, (t1, t2, t3, t4, repo) =>
                    repo.AddOrUpdate(t2, t3,
                        (oldValue, newValue) => oldValue.AddOrUpdate(t1.RegistrationId, t1, t4)),
                        registration, serviceType, newRepository, replace);
        }

        public bool ContainsRegistration(Type type, object name) =>
            this.serviceRepository.ContainsRegistration(type, name);

        public IEnumerable<KeyValuePair<Type, IServiceRegistration>> GetRegistrationMappings() =>
             this.serviceRepository.Walk().SelectMany(reg => reg.Value.Select(r => new KeyValuePair<Type, IServiceRegistration>(reg.Key, r)));

        public IServiceRegistration GetRegistrationOrDefault(Type type, ResolutionContext resolutionContext, object name = null)
        {
            var registrations = this.GetRegistrationsForType(type);
            if (registrations == null) return null;

            var filtered = this.GetTopLevelWightedRegistrations(type, name, resolutionContext, registrations, out var maxIndex);
            return filtered?[maxIndex].Key;
        }

        public IServiceRegistration GetRegistrationOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var registrations = this.GetRegistrationsForType(typeInfo.Type);
            if (registrations == null) return null;

            var filtered = this.GetWightedRegistrations(typeInfo, resolutionContext, registrations, out var maxIndex);
            return filtered?[maxIndex].Key;
        }

        public IEnumerable<IServiceRegistration> GetRegistrationsOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var registrations = this.GetRegistrationsForType(typeInfo.Type);
            if (registrations == null) return null;

            var filtered = this.GetWightedRegistrations(typeInfo, resolutionContext, registrations, out var _);
            return filtered?.Select(reg => reg.Key);
        }

        private ImmutableArray<object, IServiceRegistration> GetRegistrationsForType(Type type)
        {
            var registrations = this.serviceRepository.GetOrDefault(type);
            if (registrations == null && type.IsClosedGenericType())
                return this.serviceRepository.GetOrDefault(type.GetGenericTypeDefinition());

            return registrations;
        }

        private ArrayList<KeyValuePair<IServiceRegistration, int>> GetWightedRegistrations(
            TypeInformation typeInformation,
            ResolutionContext resolutionContext,
            ImmutableArray<object, IServiceRegistration> registrations,
            out int maxIndex)
        {
            maxIndex = 0;
            var length = registrations.Length;
            var result = new ArrayList<KeyValuePair<IServiceRegistration, int>>(length);
            var isOpenGeneric = typeInformation.Type.IsOpenGenericType();
            var maxWeight = 0;

            for (var i = 0; i < length; i++)
            {
                var weight = 0;
                var current = registrations.Repository[i];

                if (isOpenGeneric && !current.Value.ValidateGenericConstraints(typeInformation.Type))
                    continue;

                if (!this.CheckName(typeInformation.DependencyName, current, out var increaseWeight))
                    continue;

                if (increaseWeight)
                    weight++;

                if (this.CheckScopeNames(current, resolutionContext))
                    weight++;

                if (current.Value.HasCondition && current.Value.IsUsableForCurrentContext(typeInformation))
                    weight++;

                result.Add(new KeyValuePair<IServiceRegistration, int>(current.Value, weight));

                if (weight < maxWeight) continue;
                maxWeight = weight;
                maxIndex = result.Length - 1;
            }

            return result;
        }

        private ArrayList<KeyValuePair<IServiceRegistration, int>> GetTopLevelWightedRegistrations(
            Type type,
            object name,
            ResolutionContext resolutionContext,
            ImmutableArray<object, IServiceRegistration> registrations,
            out int maxIndex)
        {
            maxIndex = 0;
            var length = registrations.Length;
            var result = new ArrayList<KeyValuePair<IServiceRegistration, int>>(length);
            var isOpenGeneric = type.IsOpenGenericType();
            var maxWeight = 0;

            for (var i = 0; i < length; i++)
            {
                var weight = 0;
                var current = registrations.Repository[i];

                if (isOpenGeneric && !current.Value.ValidateGenericConstraints(type))
                    continue;

                if (!this.CheckName(name, current, out var increaseWeight))
                    continue;

                if (increaseWeight)
                    weight++;

                if (this.CheckScopeNames(current, resolutionContext))
                    weight++;

                result.Add(new KeyValuePair<IServiceRegistration, int>(current.Value, weight));

                if (weight < maxWeight) continue;
                maxWeight = weight;
                maxIndex = result.Length - 1;
            }

            return result;
        }

        private bool CheckName(object name, KeyValue<object, IServiceRegistration> current, out bool increaseWeight)
        {
            increaseWeight = false;

            if (name != null &&
                !current.Value.HasName &&
                !this.containerConfiguration.NamedDependencyResolutionForUnNamedRequestsEnabled &&
                !this.containerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled)
                return false;

            if (name == null &&
                !current.Value.HasName)
                increaseWeight = true;

            if (name != null &&
                current.Key.Equals(name))
                increaseWeight = true;

            return true;
        }

        private bool CheckScopeNames(KeyValue<object, IServiceRegistration> current, ResolutionContext resolutionContext) =>
            resolutionContext.ScopeNames != null &&
                   current.Value.HasScopeName &&
                   current.Value.CanInjectIntoNamedScope(resolutionContext.ScopeNames);
    }
}
