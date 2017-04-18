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
        private readonly ConcurrentTree<Type, ConcurrentOrderedKeyStore<string, IServiceRegistration>> serviceRepository;
        private readonly ConcurrentTree<Type, ConcurrentOrderedKeyStore<string, IServiceRegistration>> conditionalRepository;

        public RegistrationRepository()
        {
            this.serviceRepository = new ConcurrentTree<Type, ConcurrentOrderedKeyStore<string, IServiceRegistration>>();
            this.conditionalRepository = new ConcurrentTree<Type, ConcurrentOrderedKeyStore<string, IServiceRegistration>>();
        }

        public void AddOrUpdateRegistration(IServiceRegistration registration, bool remap, bool replace)
        {
            this.AddOrUpdateRegistration(registration, remap, replace,
                registration.HasCondition ? this.conditionalRepository : this.serviceRepository);
        }

        public IServiceRegistration GetRegistrationOrDefault(Type type, string name = null) =>
            name != null ? this.GetNamedRegistrationOrDefault(type, name) : this.GetDefaultRegistrationOrDefault(type);

        public IServiceRegistration GetRegistrationOrDefault(TypeInformation typeInfo, bool checkConditions = false)
        {
            if (typeInfo.DependencyName != null)
                return this.GetNamedRegistrationOrDefault(typeInfo.Type, typeInfo.DependencyName);

            return checkConditions ?
                this.GetMatchingRegistrationOrDefault(typeInfo) :
                    this.GetDefaultRegistrationOrDefault(typeInfo.Type);
        }

        public IEnumerable<IServiceRegistration> GetRegistrationsOrDefault(Type type)
        {
            var conditional = this.GetRegistrationsOrDefault(type, this.conditionalRepository);
            var registrations = this.GetRegistrationsOrDefault(type, this.serviceRepository);

            return conditional?.Concat(registrations) ?? registrations;
        }

        public IEnumerable<IServiceRegistration> GetAllRegistrations() =>
            this.conditionalRepository.SelectMany(reg => reg).Concat(this.serviceRepository.SelectMany(reg => reg));

        public bool ContainsRegistration(Type type, string name) =>
            this.ContainsRegistration(type, name, this.serviceRepository) || this.ContainsRegistration(type, name, this.conditionalRepository);

        private bool ContainsRegistration(Type type, string name, ConcurrentTree<Type, ConcurrentOrderedKeyStore<string, IServiceRegistration>> repository)
        {
            var registrations = repository.GetOrDefault(type);
            if (name != null && registrations != null)
                return registrations.GetOrDefault(name) != null;

            if (registrations != null || !type.IsClosedGenericType()) return registrations != null;

            registrations = repository.GetOrDefault(type.GetGenericTypeDefinition());
            return registrations?.Any(reg => reg.ValidateGenericContraints(type)) ?? false;
        }

        private void AddOrUpdateRegistration(IServiceRegistration registration, bool remap, bool replace, ConcurrentTree<Type, ConcurrentOrderedKeyStore<string, IServiceRegistration>> repository)
        {
            var newRepository = new ConcurrentOrderedKeyStore<string, IServiceRegistration>();
            newRepository.AddOrUpdate(registration.RegistrationContext.Name, registration);

            if (remap)
                repository.AddOrUpdate(registration.ServiceType, newRepository, (oldValue, newValue) => newValue);
            else
                repository.AddOrUpdate(registration.ServiceType, newRepository,
                    (oldValue, newValue) => oldValue.AddOrUpdate(registration.RegistrationContext.Name, registration, replace));
        }

        private IServiceRegistration GetNamedRegistrationOrDefault(Type type, string dependencyName)
        {
            var registrations = this.GetDefaultRegistrationsOrDefault(type, this.serviceRepository);
            return registrations?.GetOrDefault(dependencyName);
        }

        private IServiceRegistration GetMatchingRegistrationOrDefault(TypeInformation typeInfo)
        {
            var type = typeInfo.Type;
            var registrations = this.GetDefaultRegistrationsOrDefault(type, this.conditionalRepository);
            if (registrations == null) return this.GetDefaultRegistrationOrDefault(type);

            return registrations.Lenght > 0 ?
                registrations.FirstOrDefault(reg => reg.IsUsableForCurrentContext(typeInfo) && reg.ValidateGenericContraints(type)) :
                    registrations.Last;
        }

        private IServiceRegistration GetDefaultRegistrationOrDefault(Type type)
        {
            var registrations = this.GetDefaultRegistrationsOrDefault(type, this.serviceRepository);
            if (registrations == null) return null;

            return registrations.Lenght > 0 && type.IsClosedGenericType() ?
                registrations.FirstOrDefault(reg => reg.ValidateGenericContraints(type)) :
                    registrations.Last;
        }

        private ConcurrentOrderedKeyStore<string, IServiceRegistration> GetDefaultRegistrationsOrDefault(Type type, ConcurrentTree<Type, ConcurrentOrderedKeyStore<string, IServiceRegistration>> repository)
        {
            var registrations = repository.GetOrDefault(type);
            if (registrations == null && type.IsClosedGenericType())
                return repository.GetOrDefault(type.GetGenericTypeDefinition());

            return registrations;
        }

        private IEnumerable<IServiceRegistration> GetRegistrationsOrDefault(Type type, ConcurrentTree<Type, ConcurrentOrderedKeyStore<string, IServiceRegistration>> repository)
        {
            var registrations = repository.GetOrDefault(type);
            if (registrations == null && type.IsClosedGenericType())
                return repository.GetOrDefault(type.GetGenericTypeDefinition())?.Where(reg => reg.ValidateGenericContraints(type));

            return registrations;
        }
    }
}
