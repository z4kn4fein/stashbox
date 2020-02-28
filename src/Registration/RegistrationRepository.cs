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
    internal class RegistrationRepository : IRegistrationRepository
    {
        private readonly ContainerConfiguration containerConfiguration;
        private ImmutableTree<Type, ImmutableArray<object, IServiceRegistration>> serviceRepository = ImmutableTree<Type, ImmutableArray<object, IServiceRegistration>>.Empty;
        private ImmutableTree<Type, ImmutableArray<object, IServiceRegistration>> namedScopeRepository = ImmutableTree<Type, ImmutableArray<object, IServiceRegistration>>.Empty;

        public RegistrationRepository(ContainerConfiguration containerConfiguration)
        {
            this.containerConfiguration = containerConfiguration;
        }

        public void AddOrUpdateRegistration(IServiceRegistration registration, Type serviceType, bool remap, bool replace)
        {
            if (registration.HasScopeName)
                this.AddOrUpdateRegistration(ref this.namedScopeRepository, registration, serviceType, remap, replace);
            else
                this.AddOrUpdateRegistration(ref this.serviceRepository, registration, serviceType, remap, replace);
        }

        private void AddOrUpdateRegistration(ref ImmutableTree<Type, ImmutableArray<object, IServiceRegistration>> repository, IServiceRegistration registration, Type serviceType, bool remap, bool replace)
        {
            var newRepository = new ImmutableArray<object, IServiceRegistration>(registration.RegistrationId, registration);

            if (remap)
                Swap.SwapValue(ref repository, (t1, t2, t3, t4, repo) =>
                    repo.AddOrUpdate(t1, t2, true), serviceType, newRepository, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            else
                Swap.SwapValue(ref repository, (t1, t2, t3, t4, repo) =>
                    repo.AddOrUpdate(t2, t3,
                        (oldValue, newValue) => oldValue.AddOrUpdate(t1.RegistrationId, t1, t4)),
                        registration, serviceType, newRepository, replace);
        }

        public IServiceRegistration GetRegistrationOrDefault(Type type, ResolutionContext resolutionContext, object name = null) =>
            name != null ? this.GetNamedRegistrationOrDefault(type, name, resolutionContext) : this.GetDefaultRegistrationOrDefault(type, resolutionContext);

        public IServiceRegistration GetRegistrationOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            return typeInfo.DependencyName != null ?
                this.GetNamedRegistrationOrDefault(typeInfo, resolutionContext) :
                this.GetDefaultRegistrationOrDefault(typeInfo, resolutionContext);
        }

        public IEnumerable<KeyValue<object, IServiceRegistration>> GetRegistrationsOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            if (resolutionContext.ScopeNames == null)
                return this.GetAllRegistrationsOrDefault(typeInfo);

            return this.GetAllScopedRegistrationsOrDefault(typeInfo, resolutionContext) ?? this.GetAllRegistrationsOrDefault(typeInfo);
        }

        public IEnumerable<KeyValuePair<Type, IServiceRegistration>> GetRegistrationMappings() =>
             this.serviceRepository.Walk().SelectMany(reg => reg.Value.Select(r => new KeyValuePair<Type, IServiceRegistration>(reg.Key, r)))
                .Concat(this.namedScopeRepository.Walk().SelectMany(reg => reg.Value.Select(r => new KeyValuePair<Type, IServiceRegistration>(reg.Key, r))));

        public bool ContainsRegistration(Type type, object name) =>
            this.serviceRepository.ContainsRegistration(type, name) || this.namedScopeRepository.ContainsRegistration(type, name);

        private IServiceRegistration GetNamedRegistrationOrDefault(Type type, object dependencyName, ResolutionContext resolutionContext)
        {
            if (resolutionContext.ScopeNames == null)
                return this.GetDefaultRegistrationsOrDefault(type)?.GetOrDefault(dependencyName);

            return this.GetScopedRegistrationsOrDefault(type, resolutionContext)?.GetOrDefault(dependencyName) ??
                this.GetDefaultRegistrationsOrDefault(type)?.GetOrDefault(dependencyName);
        }

        private IServiceRegistration GetNamedRegistrationOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            if (resolutionContext.ScopeNames == null)
            {
                var registrations = this.GetDefaultRegistrationsOrDefault(typeInfo.Type);
                return this.containerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled
                    ? registrations?.GetOrDefault(typeInfo.DependencyName) ?? this.GetDefaultRegistrationOrDefault(typeInfo, resolutionContext)
                    : registrations?.GetOrDefault(typeInfo.DependencyName);
            }

            if (!this.containerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled)
                return this.GetScopedRegistrationsOrDefault(typeInfo.Type, resolutionContext)?.GetOrDefault(typeInfo.DependencyName) ??
                       this.GetDefaultRegistrationsOrDefault(typeInfo.Type)?.GetOrDefault(typeInfo.DependencyName);

            var scopedRegistrations = this.GetScopedRegistrationsOrDefault(typeInfo.Type, resolutionContext);
            if (scopedRegistrations == null)
            {
                return this.GetDefaultRegistrationsOrDefault(typeInfo.Type)?.GetOrDefault(typeInfo.DependencyName) ??
                       this.GetDefaultRegistrationOrDefault(typeInfo, resolutionContext);
            }

            return scopedRegistrations.GetOrDefault(typeInfo.DependencyName) ?? this.GetDefaultRegistrationOrDefault(typeInfo, resolutionContext);
        }

        private IServiceRegistration GetDefaultRegistrationOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var registrations = this.GetDefaultOrScopedRegistrationsOrDefault(typeInfo.Type, resolutionContext);

            if (registrations == null) return null;

            if (registrations.Length == 1 && registrations.Last.IsResolvableByUnnamedRequest)
                return registrations.Last;

            var conditionals = registrations.Where(reg => reg.HasCondition);

            return typeInfo.Type.IsClosedGenericType()
                ? conditionals.LastOrDefault(reg => reg.ValidateGenericConstraints(typeInfo.Type) &&
                                                    reg.IsUsableForCurrentContext(typeInfo) &&
                                                    reg.IsResolvableByUnnamedRequest) ??
                  registrations.LastOrDefault(reg => reg.ValidateGenericConstraints(typeInfo.Type) &&
                                                     reg.IsResolvableByUnnamedRequest)

                : conditionals.LastOrDefault(reg => reg.IsUsableForCurrentContext(typeInfo) &&
                                                    reg.IsResolvableByUnnamedRequest) ??
                  registrations.LastOrDefault(reg => reg.IsResolvableByUnnamedRequest);
        }

        private IServiceRegistration GetDefaultRegistrationOrDefault(Type type, ResolutionContext resolutionContext)
        {
            var registrations = this.GetDefaultOrScopedRegistrationsOrDefault(type, resolutionContext);

            if (registrations == null) return null;

            if (registrations.Length == 1 && registrations.Last.IsResolvableByUnnamedRequest)
                return registrations.Last;

            return type.IsClosedGenericType()
                ? registrations.LastOrDefault(reg => reg.ValidateGenericConstraints(type) && reg.IsResolvableByUnnamedRequest)
                : registrations.LastOrDefault(reg => reg.IsResolvableByUnnamedRequest);
        }

        private ImmutableArray<object, IServiceRegistration> GetDefaultOrScopedRegistrationsOrDefault(Type type, ResolutionContext resolutionContext)
        {
            if (resolutionContext.ScopeNames == null)
                return this.GetDefaultRegistrationsOrDefault(type);

            return this.GetScopedRegistrationsOrDefault(type, resolutionContext) ?? this.GetDefaultRegistrationsOrDefault(type);
        }

        private ImmutableArray<object, IServiceRegistration> GetDefaultRegistrationsOrDefault(Type type)
        {
            var registrations = this.serviceRepository.GetOrDefault(type);
            if (registrations == null && type.IsClosedGenericType())
                return this.serviceRepository.GetOrDefault(type.GetGenericTypeDefinition());

            return registrations;
        }

        private ImmutableArray<object, IServiceRegistration> GetScopedRegistrationsOrDefault(Type type, ResolutionContext resolutionContext)
        {
            var scopedRegistrations = this.namedScopeRepository.GetOrDefault(type)?.WhereOrDefault(kv => kv.Value.CanInjectIntoNamedScope(resolutionContext.ScopeNames));
            if (scopedRegistrations == null && type.IsClosedGenericType())
                return this.namedScopeRepository.GetOrDefault(type.GetGenericTypeDefinition())?
                    .WhereOrDefault(kv => kv.Value.CanInjectIntoNamedScope(resolutionContext.ScopeNames));

            return scopedRegistrations;
        }

        private IEnumerable<KeyValue<object, IServiceRegistration>> GetAllRegistrationsOrDefault(TypeInformation typeInfo)
        {
            var registrations = this.serviceRepository.GetOrDefault(typeInfo.Type)?.WhereOrDefault(kv => kv.Value.IsUsableForCurrentContext(typeInfo));

            if (!typeInfo.Type.IsClosedGenericType()) return registrations?.Repository;

            var generics = this.serviceRepository.GetOrDefault(typeInfo.Type.GetGenericTypeDefinition())?.WhereOrDefault(kv => kv.Value.IsUsableForCurrentContext(typeInfo));
            return generics == null
                ? registrations?.Repository
                : registrations?.Repository.Concat(generics.Repository.Where(reg => reg.Value.ValidateGenericConstraints(typeInfo.Type)))
                      .OrderBy(reg => reg.Value.RegistrationNumber) ?? generics.Repository.Where(reg => reg.Value.ValidateGenericConstraints(typeInfo.Type));
        }

        private IEnumerable<KeyValue<object, IServiceRegistration>> GetAllScopedRegistrationsOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var registrations = this.namedScopeRepository.GetOrDefault(typeInfo.Type)?
                .WhereOrDefault(kv => kv.Value.CanInjectIntoNamedScope(resolutionContext.ScopeNames) &&
                    kv.Value.IsUsableForCurrentContext(typeInfo));

            if (!typeInfo.Type.IsClosedGenericType()) return registrations?.Repository;

            var generics = this.namedScopeRepository.GetOrDefault(typeInfo.Type.GetGenericTypeDefinition())?
                .WhereOrDefault(kv => kv.Value.CanInjectIntoNamedScope(resolutionContext.ScopeNames) &&
                    kv.Value.IsUsableForCurrentContext(typeInfo));
            return generics == null
                ? registrations?.Repository
                : registrations?.Repository.Concat(generics.Repository.Where(reg => reg.Value.ValidateGenericConstraints(typeInfo.Type)))
                      .OrderBy(reg => reg.Value.RegistrationNumber) ?? generics.Repository.Where(reg => reg.Value.ValidateGenericConstraints(typeInfo.Type));
        }
    }
}