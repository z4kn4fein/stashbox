using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.Registration
{
    internal class RegistrationRepository : IRegistrationRepository
    {
        private readonly ContainerConfiguration containerConfiguration;
        private readonly ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>> serviceRepository;
        private readonly ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>> conditionalRepository;

        public RegistrationRepository(ContainerConfiguration containerConfiguration)
        {
            this.containerConfiguration = containerConfiguration;
            this.serviceRepository = new ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>>();
            this.conditionalRepository = new ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>>();
        }

        public void AddOrUpdateRegistration(Type typeKey, string nameKey, bool canUpdate, IServiceRegistration registration)
        {
            if (registration.HasCondition)
                this.AddOrUpdateRegistration(typeKey, nameKey, canUpdate, registration, this.conditionalRepository);

            this.AddOrUpdateRegistration(typeKey, nameKey, canUpdate, registration, this.serviceRepository);
        }

        public IServiceRegistration GetRegistrationOrDefault(TypeInformation typeInfo, bool checkConditions = false)
        {
            if (typeInfo.DependencyName != null)
                return this.GetNamedRegistrationOrDefault(typeInfo);

            return checkConditions ?
                this.GetMatchingRegistrationOrDefault(typeInfo) :
                    this.GetDefaultRegistrationOrDefault(typeInfo);
        }

        public IEnumerable<IServiceRegistration> GetRegistrationsOrDefault(TypeInformation typeInfo)
        {
            var conditional = this.GetRegistrationsOrDefault(typeInfo, this.conditionalRepository);
            var registrations = this.GetRegistrationsOrDefault(typeInfo, this.serviceRepository);

            return conditional != null ? conditional.Concat(registrations) : registrations;
        }

        public IEnumerable<IServiceRegistration> GetAllRegistrations() =>
            this.conditionalRepository.SelectMany(reg => reg).Concat(this.serviceRepository.SelectMany(reg => reg));

        public bool ContainsRegistration(TypeInformation typeInfo) =>
            this.ContainsRegistration(typeInfo, this.serviceRepository) || this.ContainsRegistration(typeInfo, this.conditionalRepository);

        public void CleanUp()
        {
            foreach (var serviceRegistration in this.GetAllRegistrations())
                serviceRegistration.CleanUp();
        }

        private bool ContainsRegistration(TypeInformation typeInfo, ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>> repository)
        {
            var registrations = repository.GetOrDefault(typeInfo.Type);
            if (typeInfo.DependencyName != null && registrations != null)
                return registrations.GetOrDefault(typeInfo.DependencyName) != null;

            if (registrations == null && typeInfo.Type.IsClosedGenericType())
            {
                registrations = repository.GetOrDefault(typeInfo.Type.GetGenericTypeDefinition());
                return registrations?.Any(reg => reg.ValidateGenericContraints(typeInfo)) ?? false;
            }

            return registrations != null;
        }

        private void AddOrUpdateRegistration(Type typeKey, string nameKey, bool canUpdate, IServiceRegistration registration, ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>> repository)
        {
            var newRepository = ConcurrentTree<string, IServiceRegistration>.Create();
            newRepository.AddOrUpdate(nameKey, registration);

            if (canUpdate)
                repository.AddOrUpdate(typeKey, newRepository,
                    (oldValue, newValue) => oldValue.HasMultipleItems ? oldValue.AddOrUpdate(nameKey, registration,
                        (oldReg, newReg) => newReg) : newValue);

            repository.AddOrUpdate(typeKey, newRepository, (oldValue, newValue) => oldValue.AddOrUpdate(nameKey, registration));
        }

        private IServiceRegistration GetNamedRegistrationOrDefault(TypeInformation typeInfo)
        {
            var registrations = this.GetDefaultRegistrationsOrDefault(typeInfo, this.serviceRepository);
            return registrations?.GetOrDefault(typeInfo.DependencyName);
        }

        private IServiceRegistration GetMatchingRegistrationOrDefault(TypeInformation typeInfo)
        {
            var registrations = this.GetDefaultRegistrationsOrDefault(typeInfo, this.conditionalRepository);
            if (registrations == null) return GetDefaultRegistrationOrDefault(typeInfo);

            return registrations.HasMultipleItems ?
                this.containerConfiguration.DependencySelectionRule(registrations.Where(reg => reg.IsUsableForCurrentContext(typeInfo))) :
                    registrations.Value;
        }

        private IServiceRegistration GetDefaultRegistrationOrDefault(TypeInformation typeInfo)
        {
            var registrations = this.GetDefaultRegistrationsOrDefault(typeInfo, this.serviceRepository);
            if (registrations == null) return null;

            return registrations.HasMultipleItems ?
                this.containerConfiguration.DependencySelectionRule(registrations) :
                    registrations.Value;
        }

        private ConcurrentTree<string, IServiceRegistration> GetDefaultRegistrationsOrDefault(TypeInformation typeInfo, ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>> repository)
        {
            var registrations = repository.GetOrDefault(typeInfo.Type);
            if (registrations == null && typeInfo.Type.IsClosedGenericType())
                return repository.GetOrDefault(typeInfo.Type.GetGenericTypeDefinition());

            return registrations;
        }

        private IEnumerable<IServiceRegistration> GetRegistrationsOrDefault(TypeInformation typeInfo, ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>> repository)
        {
            var registrations = repository.GetOrDefault(typeInfo.Type);
            if (registrations == null && typeInfo.Type.IsClosedGenericType())
                return repository.GetOrDefault(typeInfo.Type.GetGenericTypeDefinition())?.Where(reg => reg.ValidateGenericContraints(typeInfo));

            return registrations;
        }
    }
}
