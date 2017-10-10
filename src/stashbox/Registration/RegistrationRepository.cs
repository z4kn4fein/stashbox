using Stashbox.Entity;
using Stashbox.Infrastructure.Registration;
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
        private readonly HashMap<Type, ConcurrentOrderedKeyStore<object, IServiceRegistration>> serviceRepository;
        private readonly HashMap<Type, ConcurrentOrderedKeyStore<object, IServiceRegistration>> namedScopeRepository;

        public RegistrationRepository()
        {
            this.serviceRepository = new HashMap<Type, ConcurrentOrderedKeyStore<object, IServiceRegistration>>();
            this.namedScopeRepository = new HashMap<Type, ConcurrentOrderedKeyStore<object, IServiceRegistration>>();
        }

        public void AddOrUpdateRegistration(IServiceRegistration registration, bool remap, bool replace)
        {
            if (registration.HasScopeName)
                this.namedScopeRepository.AddOrUpdateRegistration(registration, remap, replace);
            else
                this.serviceRepository.AddOrUpdateRegistration(registration, remap, replace);
        }

        public IServiceRegistration GetRegistrationOrDefault(Type type, ResolutionContext resolutionContext, object name = null) =>
            name != null ? this.GetNamedRegistrationOrDefault(type, name, resolutionContext) : this.GetDefaultRegistrationOrDefault(type, resolutionContext);

        public IServiceRegistration GetRegistrationOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            return typeInfo.DependencyName != null ?
                this.GetNamedRegistrationOrDefault(typeInfo.Type, typeInfo.DependencyName, resolutionContext) :
                this.GetDefaultRegistrationOrDefault(typeInfo, resolutionContext);
        }

        public IEnumerable<KeyValue<object, IServiceRegistration>> GetRegistrationsOrDefault(Type type, ResolutionContext resolutionContext)
        {
            if (resolutionContext.ScopeNames == null)
                return this.GetAllRegistrationsOrDefault(type);

            return this.GetAllScopedRegistrationsOrDefault(type, resolutionContext) ?? this.GetAllRegistrationsOrDefault(type);
        }

        public IEnumerable<IServiceRegistration> GetAllRegistrations() =>
            this.serviceRepository.SelectMany(reg => reg).Concat(this.namedScopeRepository.SelectMany(reg => reg));

        public bool ContainsRegistration(Type type, object name)
        {
            if (!this.serviceRepository.ContainsRegistration(type, name))
                return this.namedScopeRepository.ContainsRegistration(type, name);

            return true;
        }

        private IServiceRegistration GetNamedRegistrationOrDefault(Type type, object dependencyName, ResolutionContext resolutionContext)
        {
            if (resolutionContext.ScopeNames == null)
                return this.GetDefaultRegistrationsOrDefault(type)?.GetOrDefault(dependencyName);

            return this.GetScopedRegistrationsOrDefault(type, resolutionContext)?.GetOrDefault(dependencyName) ??
                this.GetDefaultRegistrationsOrDefault(type)?.GetOrDefault(dependencyName);
        }

        private IServiceRegistration GetDefaultRegistrationOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var registrations = this.GetDefaultOrScopedRegistrationsOrDefault(typeInfo.Type, resolutionContext);

            if (registrations == null) return null;

            if (registrations.Lenght == 1)
                return registrations.Last;

            var conditionals = registrations.Where(reg => reg.HasCondition);

            return typeInfo.Type.IsClosedGenericType()
                ? conditionals.LastOrDefault(reg => reg.ValidateGenericContraints(typeInfo.Type) &&
                                                    reg.IsUsableForCurrentContext(typeInfo)) ??
                  registrations.LastOrDefault(reg => reg.ValidateGenericContraints(typeInfo.Type))

                : conditionals.LastOrDefault(reg => reg.IsUsableForCurrentContext(typeInfo)) ??
                  registrations.Last;
        }

        private IServiceRegistration GetDefaultRegistrationOrDefault(Type type, ResolutionContext resolutionContext)
        {
            var registrations = this.GetDefaultOrScopedRegistrationsOrDefault(type, resolutionContext);

            if (registrations == null) return null;

            if (registrations.Lenght == 1)
                return registrations.Last;

            return type.IsClosedGenericType()
                ? registrations.LastOrDefault(reg => reg.ValidateGenericContraints(type))
                : registrations.Last;
        }

        private ConcurrentOrderedKeyStore<object, IServiceRegistration> GetDefaultOrScopedRegistrationsOrDefault(Type type, ResolutionContext resolutionContext)
        {
            if (resolutionContext.ScopeNames == null)
                return this.GetDefaultRegistrationsOrDefault(type);

            return this.GetScopedRegistrationsOrDefault(type, resolutionContext) ?? this.GetDefaultRegistrationsOrDefault(type);
        }

        private ConcurrentOrderedKeyStore<object, IServiceRegistration> GetDefaultRegistrationsOrDefault(Type type)
        {
            var registrations = this.serviceRepository.GetOrDefault(type);
            if (registrations == null && type.IsClosedGenericType())
                return this.serviceRepository.GetOrDefault(type.GetGenericTypeDefinition());

            return registrations;
        }

        private ConcurrentOrderedKeyStore<object, IServiceRegistration> GetScopedRegistrationsOrDefault(Type type, ResolutionContext resolutionContext)
        {
            var scopedRegistrations = this.namedScopeRepository.GetOrDefault(type)?.WhereOrDefault(kv => kv.Value.CanInjectIntoNamedScope(resolutionContext.ScopeNames));
            if (scopedRegistrations == null && type.IsClosedGenericType())
                return this.namedScopeRepository.GetOrDefault(type.GetGenericTypeDefinition())?
                    .WhereOrDefault(kv => kv.Value.CanInjectIntoNamedScope(resolutionContext.ScopeNames));

            return scopedRegistrations;
        }

        private IEnumerable<KeyValue<object, IServiceRegistration>> GetAllRegistrationsOrDefault(Type type)
        {
            var registrations = this.serviceRepository.GetOrDefault(type);

            if (!type.IsClosedGenericType()) return registrations?.Repository;

            var generics = this.serviceRepository.GetOrDefault(type.GetGenericTypeDefinition());
            return generics == null ? registrations?.Repository : registrations?.Repository.Concat(generics.Repository.Where(reg => reg.Value.ValidateGenericContraints(type))).OrderBy(reg => reg.Value.RegistrationNumber) ?? generics.Repository.Where(reg => reg.Value.ValidateGenericContraints(type));
        }

        private IEnumerable<KeyValue<object, IServiceRegistration>> GetAllScopedRegistrationsOrDefault(Type type, ResolutionContext resolutionContext)
        {
            var registrations = this.namedScopeRepository.GetOrDefault(type)?.WhereOrDefault(kv => kv.Value.CanInjectIntoNamedScope(resolutionContext.ScopeNames));

            if (!type.IsClosedGenericType()) return registrations?.Repository;

            var generics = this.namedScopeRepository.GetOrDefault(type.GetGenericTypeDefinition())?.WhereOrDefault(kv => kv.Value.CanInjectIntoNamedScope(resolutionContext.ScopeNames));
            return generics == null ? registrations?.Repository : registrations?.Repository.Concat(generics.Repository.Where(reg => reg.Value.ValidateGenericContraints(type))).OrderBy(reg => reg.Value.RegistrationNumber) ?? generics.Repository.Where(reg => reg.Value.ValidateGenericContraints(type));
        }
    }
}