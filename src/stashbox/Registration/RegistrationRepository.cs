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

        public IServiceRegistration GetRegistrationOrDefault(Type type, object name = null) =>
            name != null ? this.GetNamedRegistrationOrDefault(type, name) : this.GetDefaultRegistrationOrDefault(type);

        public IServiceRegistration GetRegistrationOrDefault(TypeInformation typeInfo, bool checkConditions = false)
        {
            return typeInfo.DependencyName != null ?
                this.GetNamedRegistrationOrDefault(typeInfo.Type, typeInfo.DependencyName) :
                this.GetDefaultRegistrationOrDefault(typeInfo);
        }

        public IEnumerable<KeyValue<object, IServiceRegistration>> GetRegistrationsOrDefault(Type type)
        {
            var registrations = this.serviceRepository.GetOrDefault(type);
            
            if (!type.IsClosedGenericType()) return registrations?.Repository;

            var generics = this.serviceRepository.GetOrDefault(type.GetGenericTypeDefinition());
            return generics == null ? registrations?.Repository : registrations?.Repository.Concat(generics.Repository.Where(reg => reg.Value.ValidateGenericContraints(type))).OrderBy(reg => reg.Value.RegistrationNumber) ?? generics.Repository.Where(reg => reg.Value.ValidateGenericContraints(type));
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

        private IServiceRegistration GetDefaultRegistrationOrDefault(TypeInformation typeInfo)
        {
            var registrations = this.GetDefaultRegistrationsOrDefault(typeInfo.Type);
            if (registrations == null) return null;

            if (registrations.Lenght == 1)
                return registrations.Last;

            if (typeInfo.Type.IsClosedGenericType())
            {
                var conditional = registrations.Where(reg => reg.HasCondition).LastOrDefault(reg => reg.ValidateGenericContraints(typeInfo.Type) && reg.IsUsableForCurrentContext(typeInfo));
                return conditional ??
                       registrations.LastOrDefault(reg => reg.ValidateGenericContraints(typeInfo.Type) && reg.IsUsableForCurrentContext(typeInfo));
            }
            else
            {
                var conditional = registrations.Where(reg => reg.HasCondition).LastOrDefault(reg => reg.IsUsableForCurrentContext(typeInfo));
                return conditional ?? registrations.LastOrDefault(reg => reg.IsUsableForCurrentContext(typeInfo));
            }
        }

        private IServiceRegistration GetDefaultRegistrationOrDefault(Type type)
        {
            var registrations = this.GetDefaultRegistrationsOrDefault(type);
            if (registrations == null) return null;

            if (registrations.Lenght == 1)
                return registrations.Last;

            return type.IsClosedGenericType() ?
                registrations.LastOrDefault(reg => reg.ValidateGenericContraints(type)) :
                registrations.Last;
        }

        private ConcurrentOrderedKeyStore<object, IServiceRegistration> GetDefaultRegistrationsOrDefault(Type type)
        {
            var registrations = this.serviceRepository.GetOrDefault(type);
            if (registrations == null && type.IsClosedGenericType())
                return this.serviceRepository.GetOrDefault(type.GetGenericTypeDefinition());

            return registrations;
        }
    }
}
