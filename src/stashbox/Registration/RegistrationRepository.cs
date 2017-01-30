using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a registration repository.
    /// </summary>
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly ContainerConfiguration containerConfiguration;
        private ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>> serviceRepository;
        private ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>> genericDefinitionRepository;
        private readonly object syncObject = new object();

        /// <summary>
        /// Constructs a <see cref="RegistrationRepository"/>
        /// </summary>
        public RegistrationRepository(ContainerConfiguration containerConfiguration)
        {
            this.containerConfiguration = containerConfiguration;
            this.serviceRepository = new ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>>();
            this.genericDefinitionRepository = new ConcurrentTree<Type, ConcurrentTree<string, IServiceRegistration>>();
        }

        /// <inheritdoc />
        public IEnumerable<IServiceRegistration> GetAllRegistrations() => this.serviceRepository.SelectMany(tree => tree)
                .Concat(this.genericDefinitionRepository.SelectMany(tree => tree));

        /// <inheritdoc />
        public bool TryGetRegistrationWithConditions(TypeInformation typeInfo, out IServiceRegistration registration) =>
            typeInfo.DependencyName == null ? this.TryGetByTypeKeyWithConditions(typeInfo, out registration) : this.TryGetByNamedKey(typeInfo, out registration);

        /// <inheritdoc />
        public bool TryGetRegistrationWithConditionsWithoutGenericDefinitionExtraction(TypeInformation typeInfo, out IServiceRegistration registration) =>
            typeInfo.DependencyName == null ? this.TryGetByTypeKeyWithConditionsWithoutGenericDefinitionExtraction(typeInfo, out registration) :
                this.TryGetByNamedKeyWithoutGenericDefinitionExtraction(typeInfo, out registration);

        /// <inheritdoc />
        public bool TryGetRegistration(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            return typeInfo.DependencyName == null ? this.TryGetByTypeKey(typeInfo, out registration) : this.TryGetByNamedKey(typeInfo, out registration);
        }

        /// <inheritdoc />
        public void AddRegistration(Type typeKey, IServiceRegistration registration, string nameKey)
        {
            var newTree = ConcurrentTree<string, IServiceRegistration>.Create();
            newTree.AddOrUpdate(nameKey, registration);

            this.serviceRepository.AddOrUpdate(typeKey, newTree, (oldValue, newValue) => oldValue.AddOrUpdate(nameKey, registration));
        }

        /// <inheritdoc />
        public void AddOrUpdateRegistration(Type typeKey, IServiceRegistration registration, string nameKey)
        {
            var newTree = ConcurrentTree<string, IServiceRegistration>.Create();
            newTree.AddOrUpdate(nameKey, registration);

            this.serviceRepository.AddOrUpdate(typeKey, newTree,
                (oldValue, newValue) => oldValue.HasMultipleItems ? oldValue.AddOrUpdate(nameKey, registration,
                    (oldReg, newReg) => newReg) : newValue);
        }

        /// <inheritdoc />
        public void AddGenericDefinition(Type typeKey, IServiceRegistration registration, string nameKey)
        {
            var newTree = ConcurrentTree<string, IServiceRegistration>.Create();
            newTree.AddOrUpdate(nameKey, registration);

            this.genericDefinitionRepository.AddOrUpdate(typeKey, newTree, (oldValue, newValue) => oldValue.AddOrUpdate(nameKey, registration));
        }

        /// <inheritdoc />
        public void AddOrUpdateGenericDefinition(Type typeKey, IServiceRegistration registration, string nameKey)
        {
            var newTree = ConcurrentTree<string, IServiceRegistration>.Create();
            newTree.AddOrUpdate(nameKey, registration);

            this.genericDefinitionRepository.AddOrUpdate(typeKey, newTree,
                (oldValue, newValue) => oldValue.HasMultipleItems ? oldValue.AddOrUpdate(nameKey, registration,
                    (oldReg, newReg) => newReg) : newValue);
        }

        /// <inheritdoc />
        public bool TryGetTypedRepositoryRegistrations(TypeInformation typeInfo, out IServiceRegistration[] registrations)
        {
            var serviceRegistrations = this.serviceRepository.GetOrDefault(typeInfo.Type)?.ToArray();
            if (serviceRegistrations == null)
            {
                Type genericTypeDefinition;
                if (this.TryHandleOpenGenericType(typeInfo.Type, out genericTypeDefinition))
                    serviceRegistrations = this.genericDefinitionRepository.GetOrDefault(genericTypeDefinition)?.Where(reg => reg.ValidateGenericContraints(typeInfo)).ToArray();
                else
                {
                    registrations = null;
                    return false;
                }
            }

            registrations = serviceRegistrations;
            return registrations != null;
        }

        /// <inheritdoc />
        public bool ConstainsRegistrationWithConditions(TypeInformation typeInfo)
        {
            var registrations = this.serviceRepository.GetOrDefault(typeInfo.Type);
            if (registrations != null)
                return registrations
                           .Any(registration => registration.IsUsableForCurrentContext(typeInfo) &&
                           this.CheckDependencyName(registration.RegistrationName, typeInfo.DependencyName));

            Type genericTypeDefinition;
            if (this.TryHandleOpenGenericType(typeInfo.Type, out genericTypeDefinition))
            {
                registrations = this.genericDefinitionRepository.GetOrDefault(genericTypeDefinition);
                return registrations != null && registrations.Any(registration => registration.IsUsableForCurrentContext(new TypeInformation
                {
                    Type = genericTypeDefinition,
                    ParentType = typeInfo.ParentType,
                    DependencyName = typeInfo.DependencyName,
                    CustomAttributes = typeInfo.CustomAttributes
                }) && this.CheckDependencyName(registration.RegistrationName, typeInfo.DependencyName));
            }

            if (typeInfo.Type.GetTypeInfo().IsGenericTypeDefinition)
                return this.genericDefinitionRepository.GetOrDefault(typeInfo.Type) != null;

            return false;
        }

        private bool CheckDependencyName(string key, string dependencyName)
        {
            if (dependencyName == null) return true;

            return key == dependencyName;
        }

        /// <inheritdoc />
        public void CleanUp()
        {
            foreach (var registration in this.serviceRepository.SelectMany(tree => tree))
                registration.CleanUp();

            this.serviceRepository = null;
        }

        private bool TryGetByTypeKey(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            ConcurrentTree<string, IServiceRegistration> registrations;
            if (!this.TryGetRegistrationsByType(typeInfo, out registrations))
            {
                registration = null;
                return false;
            }

            registration = registrations.HasMultipleItems ? this.containerConfiguration.DependencySelectionRule(registrations) : registrations.Value;

            return true;
        }

        private bool TryGetByTypeKeyWithConditions(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            ConcurrentTree<string, IServiceRegistration> registrations;
            if (!this.TryGetRegistrationsByType(typeInfo, out registrations))
            {
                registration = null;
                return false;
            }

            if (registrations.HasMultipleItems)
            {
                var serviceRegistrations = registrations.ToArray();
                registration = this.containerConfiguration.DependencySelectionRule(this.ApplyConditionFilters(serviceRegistrations, typeInfo));
            }
            else
                registration = registrations.Value;

            return registration != null;
        }

        private bool TryGetByTypeKeyWithConditionsWithoutGenericDefinitionExtraction(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            ConcurrentTree<string, IServiceRegistration> registrations;
            if (!this.TryGetRegistrationsByTypeWithoutGenericDefinitionExtraction(typeInfo.Type, out registrations))
            {
                registration = null;
                return false;
            }

            if (registrations.HasMultipleItems)
            {
                var serviceRegistrations = registrations.ToArray();
                registration = this.containerConfiguration.DependencySelectionRule(this.ApplyConditionFilters(serviceRegistrations, typeInfo));
            }
            else
                registration = registrations.Value;

            return registration != null;
        }

        private IEnumerable<IServiceRegistration> ApplyConditionFilters(IServiceRegistration[] registrations, TypeInformation typeInfo)
        {
            return registrations.Any(reg => reg.HasCondition)
                ? registrations.Where(reg => reg.HasCondition && reg.IsUsableForCurrentContext(typeInfo))
                : registrations.Where(reg => reg.IsUsableForCurrentContext(typeInfo));
        }

        private bool TryGetRegistrationsByType(TypeInformation typeInfo, out ConcurrentTree<string, IServiceRegistration> registrations)
        {
            var type = typeInfo.Type;
            registrations = this.serviceRepository.GetOrDefault(type);
            if (registrations != null) return true;

            Type genericTypeDefinition;
            if (this.TryHandleOpenGenericType(type, out genericTypeDefinition))
                registrations = this.genericDefinitionRepository.GetOrDefault(genericTypeDefinition);

            else if (type.GetTypeInfo().IsGenericTypeDefinition)
                registrations = this.genericDefinitionRepository.GetOrDefault(type);

            return registrations != null;
        }

        private bool TryGetRegistrationsByTypeWithoutGenericDefinitionExtraction(Type type, out ConcurrentTree<string, IServiceRegistration> registrations)
        {
            registrations = this.serviceRepository.GetOrDefault(type);
            return registrations != null;
        }

        private bool TryGetByNamedKey(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            ConcurrentTree<string, IServiceRegistration> registrations;
            if (this.TryGetRegistrationsByType(typeInfo, out registrations))
            {
                registration = registrations.GetOrDefault(typeInfo.DependencyName);
                return registration != null;
            }

            registration = null;
            return false;
        }

        private bool TryGetByNamedKeyWithoutGenericDefinitionExtraction(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            ConcurrentTree<string, IServiceRegistration> registrations;
            if (this.TryGetRegistrationsByTypeWithoutGenericDefinitionExtraction(typeInfo.Type, out registrations))
            {
                registration = registrations.GetOrDefault(typeInfo.DependencyName);
                return registration != null;
            }

            registration = null;
            return false;
        }

        private bool TryHandleOpenGenericType(Type type, out Type genericTypeDefinition)
        {
            if (type.IsConstructedGenericType)
            {
                genericTypeDefinition = type.GetGenericTypeDefinition();
                return true;
            }

            genericTypeDefinition = null;
            return false;
        }
    }
}
