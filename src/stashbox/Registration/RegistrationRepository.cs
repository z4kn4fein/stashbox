using Stashbox.Entity;
using Stashbox.Infrastructure.Registration;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.Registration
{
    internal class RegistrationRepository : IRegistrationRepository
    {
        private readonly ConcurrentTree<Type, ConcurrentOrderedKeyStore<object, IServiceRegistration>> serviceRepository;

        public RegistrationRepository()
        {
            this.serviceRepository = new ConcurrentTree<Type, ConcurrentOrderedKeyStore<object, IServiceRegistration>>();
        }

        public void AddOrUpdateRegistration(IServiceRegistration registration, bool remap, bool replace)
        {
            var newRepository = new ConcurrentOrderedKeyStore<object, IServiceRegistration>();
            newRepository.AddOrUpdate(registration.RegistrationContext.Name, registration);

            if (remap)
                this.serviceRepository.AddOrUpdate(registration.ServiceType, newRepository, (oldValue, newValue) => newValue);
            else
                this.serviceRepository.AddOrUpdate(registration.ServiceType, newRepository,
                    (oldValue, newValue) => oldValue.AddOrUpdate(registration.RegistrationContext.Name, registration, replace));
        }

        public IServiceRegistration GetRegistrationOrDefault(Type type, object scopeName, object name = null) =>
            name != null ? this.GetNamedRegistrationOrDefault(type, name) : this.GetDefaultRegistrationOrDefault(type, scopeName);

        public IServiceRegistration GetRegistrationOrDefault(TypeInformation typeInfo, object scopeName)
        {
            return typeInfo.DependencyName != null ?
                this.GetNamedRegistrationOrDefault(typeInfo.Type, typeInfo.DependencyName) :
                this.GetDefaultRegistrationOrDefault(typeInfo, scopeName);
        }

        public IEnumerable<KeyValue<object, IServiceRegistration>> GetRegistrationsOrDefault(Type type, object scopeName)
        {
            var registrations = this.serviceRepository.GetOrDefault(type);

            if (!type.IsClosedGenericType()) return GetRegistrationsFromCollection(registrations, scopeName);

            var generics = this.serviceRepository.GetOrDefault(type.GetGenericTypeDefinition());
            return generics == null
                ? GetRegistrationsFromCollection(registrations, scopeName)
                : GetGenericRegistrationsFromCollection(registrations, generics, type, scopeName);
        }

        public IEnumerable<IServiceRegistration> GetAllRegistrations() =>
            this.serviceRepository.SelectMany(reg => reg);

        public bool ContainsRegistration(Type type, object name)
        {
            var registrations = this.serviceRepository.GetOrDefault(type);
            if (name != null && registrations != null)
                return registrations.GetOrDefault(name) != null;

            if (registrations != null || !type.IsClosedGenericType()) return registrations != null;

            registrations = this.serviceRepository.GetOrDefault(type.GetGenericTypeDefinition());
            return registrations?.Any(reg => reg.ValidateGenericContraints(type)) ?? false;
        }

        private IServiceRegistration GetNamedRegistrationOrDefault(Type type, object dependencyName)
        {
            var registrations = this.GetDefaultRegistrationsOrDefault(type);
            return registrations?.GetOrDefault(dependencyName);
        }

        private IServiceRegistration GetDefaultRegistrationOrDefault(TypeInformation typeInfo, object scopeName)
        {
            var registrations = scopeName == null
                ? this.GetDefaultRegistrationsOrDefault(typeInfo.Type)
                : this.GetScopedRegistrationsOrDefault(typeInfo.Type, scopeName);

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

        private IServiceRegistration GetDefaultRegistrationOrDefault(Type type, object scopeName)
        {
            var registrations = scopeName == null
                ? this.GetDefaultRegistrationsOrDefault(type)
                : this.GetScopedRegistrationsOrDefault(type, scopeName);

            if (registrations == null) return null;

            if (registrations.Lenght == 1)
                return registrations.Last;

            return type.IsClosedGenericType()
                ? registrations.LastOrDefault(reg => reg.ValidateGenericContraints(type))
                : registrations.Last;
        }

        private ConcurrentOrderedKeyStore<object, IServiceRegistration> GetDefaultRegistrationsOrDefault(Type type)
        {
            var registrations = this.serviceRepository.GetOrDefault(type);
            if (registrations == null && type.IsClosedGenericType())
                return this.serviceRepository.GetOrDefault(type.GetGenericTypeDefinition());

            return registrations;
        }

        private ConcurrentOrderedKeyStore<object, IServiceRegistration> GetScopedRegistrationsOrDefault(Type type, object scopeName)
        {
            var registrations = this.serviceRepository.GetOrDefault(type);
            var scopedRegistrations = registrations?.WhereOrDefault(kv => kv.Value.RegistrationContext.UsedScopeName == scopeName);
            if (scopedRegistrations == null && type.IsClosedGenericType())
            {
                var definitions = this.serviceRepository.GetOrDefault(type.GetGenericTypeDefinition())?
                    .WhereOrDefault(kv => kv.Value.RegistrationContext.UsedScopeName == scopeName);

                return definitions ?? registrations;
            }

            return scopedRegistrations ?? registrations;
        }

        private static IEnumerable<KeyValue<object, IServiceRegistration>> GetRegistrationsFromCollection(ConcurrentOrderedKeyStore<object, IServiceRegistration> collection, object scopeName)
        {
            if (collection == null)
                return null;

            if (scopeName == null)
                return collection.Repository;

            return collection.WhereOrDefault(reg => reg.Value.RegistrationContext.UsedScopeName == scopeName)?.Repository ?? collection.Repository;
        }

        private static IEnumerable<KeyValue<object, IServiceRegistration>> GetGenericRegistrationsFromCollection(ConcurrentOrderedKeyStore<object, IServiceRegistration> collection, ConcurrentOrderedKeyStore<object, IServiceRegistration> generics, Type type, object scopeName)
        {
            if (scopeName == null)
                return collection?.Repository.Concat(generics.Repository.Where(reg => reg.Value.ValidateGenericContraints(type))).OrderBy(reg => reg.Value.RegistrationNumber) ?? generics.Repository.Where(reg => reg.Value.ValidateGenericContraints(type));

            var filtered = generics.WhereOrDefault(reg => reg.Value.ValidateGenericContraints(type) &&
                                                          reg.Value.RegistrationContext.UsedScopeName == scopeName);

            var genericFiltered = filtered == null ? generics.Repository.Where(reg => reg.Value.ValidateGenericContraints(type)) : filtered.Repository;

            if (collection == null)
                return genericFiltered;

            var registrations = GetRegistrationsFromCollection(collection, scopeName);

            return registrations?.Concat(genericFiltered).OrderBy(reg => reg.Value.RegistrationNumber) ?? genericFiltered;
        }
    }
}