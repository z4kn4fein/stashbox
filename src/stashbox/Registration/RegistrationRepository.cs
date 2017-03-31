using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.Registration
{
    internal class RegistrationRepository : IRegistrationRepository
    {
        private readonly IContainerConfigurator containerConfigurator;
        private readonly ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>> serviceRepository;
        private readonly ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>> conditionalRepository;

        public RegistrationRepository(IContainerConfigurator containerConfigurator)
        {
            this.containerConfigurator = containerConfigurator;
            this.serviceRepository = new ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>>();
            this.conditionalRepository = new ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>>();
        }

        public void AddOrUpdateRegistration(Type type, string name, bool canUpdate, IServiceRegistration registration)
        {
            this.AddOrUpdateRegistration(type, name, canUpdate, registration,
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
        
        private bool ContainsRegistration(Type type, string name, ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>> repository)
        {
            var registrations = repository.GetOrDefault(type);
            if (name != null && registrations != null)
                return registrations.GetOrDefault(name) != null;

            if (registrations != null || !type.IsClosedGenericType()) return registrations != null;

            registrations = repository.GetOrDefault(type.GetGenericTypeDefinition());
            return registrations?.Any(reg => reg.ValidateGenericContraints(type)) ?? false;
        }

        private void AddOrUpdateRegistration(Type type, string name, bool canUpdate, IServiceRegistration registration, ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>> repository)
        {
            var newRepository = ConcurrentTree<string, IServiceRegistration>.Create();
            newRepository.AddOrUpdate(name, registration);

            if (canUpdate)
                repository.AddOrUpdate(type, newRepository,
                    (oldValue, newValue) => oldValue.HasMultipleItems ? oldValue.AddOrUpdate(name, registration,
                        (oldReg, newReg) => newReg) : newValue);
            else
                repository.AddOrUpdate(type, newRepository, (oldValue, newValue) => oldValue.AddOrUpdate(name, registration));
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

            return registrations.HasMultipleItems ?
                this.containerConfigurator.ContainerConfiguration.DependencySelectionRule(registrations.Where(reg => reg.IsUsableForCurrentContext(typeInfo) && reg.ValidateGenericContraints(type))) :
                    registrations.Value;
        }

        private IServiceRegistration GetDefaultRegistrationOrDefault(Type type)
        {
            var registrations = this.GetDefaultRegistrationsOrDefault(type, this.serviceRepository);
            if (registrations == null) return null;

            return registrations.HasMultipleItems ?
                this.containerConfigurator.ContainerConfiguration.DependencySelectionRule(registrations.Where(reg => reg.ValidateGenericContraints(type))) :
                    registrations.Value;
        }

        private ConcurrentTree<string, IServiceRegistration> GetDefaultRegistrationsOrDefault(Type type, ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>> repository)
        {
            var registrations = repository.GetOrDefault(type);
            if (registrations == null && type.IsClosedGenericType())
                return repository.GetOrDefault(type.GetGenericTypeDefinition());

            return registrations;
        }

        private IEnumerable<IServiceRegistration> GetRegistrationsOrDefault(Type type, ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>> repository)
        {
            var registrations = repository.GetOrDefault(type);
            if (registrations == null && type.IsClosedGenericType())
                return repository.GetOrDefault(type.GetGenericTypeDefinition())?.Where(reg => reg.ValidateGenericContraints(type));

            return registrations;
        }
    }
}
