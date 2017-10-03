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
            var registrations = this.GetDefaultRegistrationsOrDefault(typeInfo.Type);
            if (registrations == null) return null;

            if (registrations.Lenght == 1)
                return registrations.Last;

            var conditionals = registrations.Where(reg => reg.HasCondition);

            return typeInfo.Type.IsClosedGenericType()
                ? GetGenericRegistrationFromCollection(conditionals, typeInfo, scopeName) ?? GetGenericRegistrationFromCollection(registrations, typeInfo, scopeName)
                : GetRegistrationFromCollection(conditionals, typeInfo, scopeName) ?? GetRegistrationFromCollection(registrations, typeInfo, scopeName);
        }

        private IServiceRegistration GetDefaultRegistrationOrDefault(Type type, object scopeName)
        {
            var registrations = this.GetDefaultRegistrationsOrDefault(type);
            if (registrations == null) return null;

            if (registrations.Lenght == 1)
                return registrations.Last;

            return type.IsClosedGenericType()
                ? GetGenericRegistrationFromCollection(registrations, type, scopeName)
                : GetRegistrationFromCollection(registrations, scopeName);
        }

        private ConcurrentOrderedKeyStore<object, IServiceRegistration> GetDefaultRegistrationsOrDefault(Type type)
        {
            var registrations = this.serviceRepository.GetOrDefault(type);
            if (registrations == null && type.IsClosedGenericType())
                return this.serviceRepository.GetOrDefault(type.GetGenericTypeDefinition());

            return registrations;
        }

        private static IEnumerable<KeyValue<object, IServiceRegistration>> GetRegistrationsFromCollection(ConcurrentOrderedKeyStore<object, IServiceRegistration> collection, object scopeName)
        {
            if (collection == null)
                return null;

            if (scopeName == null)
                return collection.Repository;

            var filtered = collection.Repository.Where(reg => reg.Value.RegistrationContext.UsedScopeName == scopeName);
            return !filtered.Any() ? collection.Repository : filtered;
        }

        private static IEnumerable<KeyValue<object, IServiceRegistration>> GetGenericRegistrationsFromCollection(ConcurrentOrderedKeyStore<object, IServiceRegistration> collection, ConcurrentOrderedKeyStore<object, IServiceRegistration> generics, Type type, object scopeName)
        {
            if (scopeName == null)
                return collection?.Repository.Concat(generics.Repository.Where(reg => reg.Value.ValidateGenericContraints(type))).OrderBy(reg => reg.Value.RegistrationNumber) ?? generics.Repository.Where(reg => reg.Value.ValidateGenericContraints(type));

            var filtered = generics.Repository.Where(reg => reg.Value.ValidateGenericContraints(type) &&
                                reg.Value.RegistrationContext.UsedScopeName == scopeName);

            var genericFiltered = !filtered.Any() ? generics.Repository.Where(reg => reg.Value.ValidateGenericContraints(type)) : filtered;

            if (collection == null)
                return genericFiltered;

            var registrations = GetRegistrationsFromCollection(collection, scopeName);

            return registrations?.Concat(genericFiltered).OrderBy(reg => reg.Value.RegistrationNumber) ?? genericFiltered;
        }

        private static IServiceRegistration GetGenericRegistrationFromCollection(ConcurrentOrderedKeyStore<object, IServiceRegistration> collection, Type type, object scopeName) =>
            scopeName == null
                ? collection.LastOrDefault(reg => reg.ValidateGenericContraints(type))
                : collection.LastOrDefault(reg => reg.ValidateGenericContraints(type) && reg.RegistrationContext.UsedScopeName == scopeName) ??
                    collection.LastOrDefault(reg => reg.ValidateGenericContraints(type));

        private static IServiceRegistration GetRegistrationFromCollection(ConcurrentOrderedKeyStore<object, IServiceRegistration> collection, object scopeName) =>
            scopeName == null
                ? collection.Last
                : collection.LastOrDefault(reg => reg.RegistrationContext.UsedScopeName == scopeName) ?? collection.Last;

        private static IServiceRegistration GetGenericRegistrationFromCollection(IEnumerable<IServiceRegistration> collection, TypeInformation typeInfo, object scopeName) =>
            scopeName == null
                ? collection.LastOrDefault(reg => reg.ValidateGenericContraints(typeInfo.Type) &&
                                                  reg.IsUsableForCurrentContext(typeInfo))
                : collection.LastOrDefault(reg => reg.ValidateGenericContraints(typeInfo.Type) &&
                                                  reg.IsUsableForCurrentContext(typeInfo) &&
                                                  reg.RegistrationContext.UsedScopeName == scopeName) ??
                    collection.LastOrDefault(reg => reg.ValidateGenericContraints(typeInfo.Type) &&
                                                    reg.IsUsableForCurrentContext(typeInfo));

        private static IServiceRegistration GetRegistrationFromCollection(IEnumerable<IServiceRegistration> collection, TypeInformation typeInfo, object scopeName) =>
            scopeName == null
                ? collection.LastOrDefault(reg => reg.IsUsableForCurrentContext(typeInfo))
                : collection.LastOrDefault(reg => reg.IsUsableForCurrentContext(typeInfo) &&
                                                  reg.RegistrationContext.UsedScopeName == scopeName) ??
                    collection.LastOrDefault(reg => reg.IsUsableForCurrentContext(typeInfo));
    }
}
