using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a registration repository.
    /// </summary>
    public class RegistrationRepository : IRegistrationRepository
    {
        private ImmutableTree<ImmutableTree<IServiceRegistration>> serviceRepository;
        private ImmutableTree<ImmutableTree<IServiceRegistration>> genericDefinitionRepository;
        private readonly object syncObject = new object();

        /// <summary>
        /// Constructs a <see cref="RegistrationRepository"/>
        /// </summary>
        public RegistrationRepository()
        {
            this.serviceRepository = ImmutableTree<ImmutableTree<IServiceRegistration>>.Empty;
            this.genericDefinitionRepository = ImmutableTree<ImmutableTree<IServiceRegistration>>.Empty;
        }

        /// <summary>
        /// Gets all registrations.
        /// </summary>
        /// <returns>A collection of the registrations.</returns>
        public IEnumerable<IServiceRegistration> GetAllRegistrations()
        {
            return this.serviceRepository.Enumerate().SelectMany(tree => tree.Value.Enumerate().Select(reg => reg.Value))
                .Concat(this.genericDefinitionRepository.Enumerate().SelectMany(tree => tree.Value.Enumerate().Select(reg => reg.Value)));
        }

        /// <summary>
        /// Tries to retrieve a registration with conditions.
        /// </summary>
        /// <param name="typeInfo">The requested type information.</param>
        /// <param name="registration">The retrieved registration.</param>
        /// <returns>True if a registration was found, otherwise false.</returns>
        public bool TryGetRegistrationWithConditions(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            return typeInfo.DependencyName == null ? this.TryGetByTypeKeyWithConditions(typeInfo, out registration) : this.TryGetByNamedKey(typeInfo, out registration);
        }

        /// <summary>
        /// Tries to retrieve a non generic definition registration with conditions.
        /// </summary>
        /// <param name="typeInfo">The requested type information.</param>
        /// <param name="registration">The retrieved registration.</param>
        /// <returns>True if a registration was found, otherwise false.</returns>
        public bool TryGetRegistrationWithConditionsWithoutGenericDefinitionExtraction(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            return typeInfo.DependencyName == null ? this.TryGetByTypeKeyWithConditionsWithoutGenericDefinitionExtraction(typeInfo, out registration) :
                this.TryGetByNamedKeyWithoutGenericDefinitionExtraction(typeInfo, out registration);
        }

        /// <summary>
        /// Tries to retrieve a registrations.
        /// </summary>
        /// <param name="typeInfo">The requested type information.</param>
        /// <param name="registration">The retrieved registration.</param>
        /// <returns>True if a registration was found, otherwise false.</returns>
        public bool TryGetRegistration(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            return typeInfo.DependencyName == null ? this.TryGetByTypeKey(typeInfo, out registration) : this.TryGetByNamedKey(typeInfo, out registration);
        }

        /// <summary>
        /// Adds a new registration into the repository.
        /// </summary>
        /// <param name="typeKey">The key type.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="nameKey">The name of the registration.</param>
        public void AddRegistration(Type typeKey, IServiceRegistration registration, string nameKey)
        {
            var immutableTree = ImmutableTree<IServiceRegistration>.Empty;
            var newTree = immutableTree.AddOrUpdate(nameKey.GetHashCode(), registration);

            lock (this.syncObject)
            {
                this.serviceRepository = this.serviceRepository.AddOrUpdate(typeKey.GetHashCode(), newTree, (oldValue, newValue) =>
                {
                    return oldValue.AddOrUpdate(nameKey.GetHashCode(), registration, (oldRegistration, newReg) => oldRegistration);
                });
            }
        }

        /// <summary>
        /// Adds or updates an element in the repository.
        /// </summary>
        /// <param name="typeKey">The key type.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="nameKey">The name of the registration.</param>
        public void AddOrUpdateRegistration(Type typeKey, IServiceRegistration registration, string nameKey)
        {
            var immutableTree = ImmutableTree<IServiceRegistration>.Empty;
            var newTree = immutableTree.AddOrUpdate(nameKey.GetHashCode(), registration);

            lock (this.syncObject)
            {
                this.serviceRepository = this.serviceRepository.AddOrUpdate(typeKey.GetHashCode(), newTree, (oldValue, newValue) => newValue);
            }
        }

        /// <summary>
        /// Adds a new generic definition into the repository.
        /// </summary>
        /// <param name="typeKey">The key type.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="nameKey">The name of the registration.</param>
        public void AddGenericDefinition(Type typeKey, IServiceRegistration registration, string nameKey)
        {
            var immutableTree = ImmutableTree<IServiceRegistration>.Empty;
            var newTree = immutableTree.AddOrUpdate(nameKey.GetHashCode(), registration);

            lock (this.syncObject)
            {
                this.genericDefinitionRepository = this.genericDefinitionRepository.AddOrUpdate(typeKey.GetHashCode(), newTree, (oldValue, newValue) =>
                {
                    return oldValue.AddOrUpdate(nameKey.GetHashCode(), registration, (oldRegistration, newReg) => oldRegistration);
                });
            }
        }

        /// <summary>
        /// Adds or updates a generic definition in the repository.
        /// </summary>
        /// <param name="typeKey">The key type.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="nameKey">The name of the registration.</param>
        public void AddOrUpdateGenericDefinition(Type typeKey, IServiceRegistration registration, string nameKey)
        {
            var immutableTree = ImmutableTree<IServiceRegistration>.Empty;
            var newTree = immutableTree.AddOrUpdate(nameKey.GetHashCode(), registration);

            lock (this.syncObject)
            {
                this.genericDefinitionRepository = this.genericDefinitionRepository.AddOrUpdate(typeKey.GetHashCode(), newTree, (oldValue, newValue) => newValue);
            }
        }

        /// <summary>
        /// Tries to retrieve all registrations for a type.
        /// </summary>
        /// <param name="typeInfo">The requested type information.</param>
        /// <param name="registrations">The retrieved registrations.</param>
        /// <returns>True if registrations were found, otherwise false.</returns>
        public bool TryGetTypedRepositoryRegistrations(TypeInformation typeInfo, out IServiceRegistration[] registrations)
        {
            var serviceRegistrations = this.serviceRepository.GetValueOrDefault(typeInfo.Type.GetHashCode());
            if (serviceRegistrations == null)
            {
                Type genericTypeDefinition;
                if (this.TryHandleOpenGenericType(typeInfo.Type, out genericTypeDefinition))
                {
                    serviceRegistrations = this.genericDefinitionRepository.GetValueOrDefault(genericTypeDefinition.GetHashCode());
                }
                else
                {
                    registrations = null;
                    return false;
                }
            }

            registrations = serviceRegistrations?.Enumerate().Select(reg => reg.Value).ToArray();
            return registrations != null;
        }

        /// <summary>
        /// Checks a type exists in the repository.
        /// </summary>
        /// <param name="type">The requested type.</param>
        /// <param name="name">The registration name.</param>
        /// <returns>True if the registration found, otherwise false.</returns>
        public bool Constains(Type type, string name = null)
        {
            var regs = this.serviceRepository.GetValueOrDefault(type.GetHashCode());
            if (name != null)
                return regs != null && regs.GetValueOrDefault(name.GetHashCode()) != null;
            else
                return regs != null;
        }

        /// <summary>
        /// Check a type exists with conditions.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>True if the registration found, otherwise false.</returns>
        public bool ConstainsTypeKeyWithConditions(TypeInformation typeInfo)
        {
            var registrations = this.serviceRepository.GetValueOrDefault(typeInfo.Type.GetHashCode());
            if (registrations != null)
                return registrations.Value != null &&
                       registrations.Enumerate()
                           .Any(registration => registration.Value.IsUsableForCurrentContext(typeInfo));
            {
                Type genericTypeDefinition;
                if (!this.TryHandleOpenGenericType(typeInfo.Type, out genericTypeDefinition)) return false;
                registrations = this.genericDefinitionRepository.GetValueOrDefault(genericTypeDefinition.GetHashCode());
                return registrations != null && registrations.Enumerate().Any(registration => registration.Value.IsUsableForCurrentContext(new TypeInformation
                {
                    Type = genericTypeDefinition,
                    ParentType = typeInfo.ParentType,
                    DependencyName = typeInfo.DependencyName,
                    CustomAttributes = typeInfo.CustomAttributes
                }));
            }
        }

        /// <summary>
        /// Checks a non generic definition type exists in the repository.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>True if the registration found, otherwise false.</returns>
        public bool ConstainsTypeKeyWithConditionsWithoutGenericDefinitionExtraction(TypeInformation typeInfo)
        {
            var registrations = this.serviceRepository.GetValueOrDefault(typeInfo.Type.GetHashCode());
            if (registrations == null) return false;
            return registrations.Value != null && registrations.Enumerate().Any(registration => registration.Value.IsUsableForCurrentContext(typeInfo));
        }

        /// <summary>
        /// Cleans up the repository.
        /// </summary>
        public void CleanUp()
        {
            foreach (var registration in this.serviceRepository.Enumerate().Select(reg => reg.Value).SelectMany(registrations => registrations.Enumerate()))
            {
                registration.Value.CleanUp();
            }

            this.serviceRepository = null;
        }

        private bool TryGetByTypeKey(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            ImmutableTree<IServiceRegistration> registrations;
            if (!this.TryGetRegistrationsByType(typeInfo.Type, out registrations))
            {
                registration = null;
                return false;
            }

            registration = registrations.Value;
            return true;
        }

        private bool TryGetByTypeKeyWithConditions(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            ImmutableTree<IServiceRegistration> registrations;
            if (!this.TryGetRegistrationsByType(typeInfo.Type, out registrations))
            {
                registration = null;
                return false;
            }

            var serviceRegistrations = registrations.Enumerate().Select(reg => reg.Value).ToArray();
            if (serviceRegistrations.Any(reg => reg.HasCondition))
                registration = serviceRegistrations.Where(reg => reg.HasCondition)
                                                   .FirstOrDefault(reg => reg.IsUsableForCurrentContext(typeInfo));
            else
                registration = serviceRegistrations.FirstOrDefault(reg => reg.IsUsableForCurrentContext(typeInfo));

            return registration != null;
        }

        private bool TryGetByTypeKeyWithConditionsWithoutGenericDefinitionExtraction(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            ImmutableTree<IServiceRegistration> registrations;
            if (!this.TryGetRegistrationsByTypeWithoutGenericDefinitionExtraction(typeInfo.Type, out registrations))
            {
                registration = null;
                return false;
            }

            var serviceRegistrations = registrations.Enumerate().Select(reg => reg.Value).ToArray();
            if (serviceRegistrations.Any(reg => reg.HasCondition))
                registration = serviceRegistrations.Where(reg => reg.HasCondition)
                                                   .FirstOrDefault(reg => reg.IsUsableForCurrentContext(typeInfo));
            else
                registration = serviceRegistrations.FirstOrDefault(reg => reg.IsUsableForCurrentContext(typeInfo));

            return registration != null;
        }

        private bool TryGetRegistrationsByType(Type type, out ImmutableTree<IServiceRegistration> registrations)
        {
            registrations = this.serviceRepository.GetValueOrDefault(type.GetHashCode());
            if (registrations != null) return true;
            Type genericTypeDefinition;
            if (this.TryHandleOpenGenericType(type, out genericTypeDefinition))
            {
                registrations = this.genericDefinitionRepository.GetValueOrDefault(genericTypeDefinition.GetHashCode());
            }
            else
            {
                return false;
            }

            return registrations != null;
        }

        private bool TryGetRegistrationsByTypeWithoutGenericDefinitionExtraction(Type type, out ImmutableTree<IServiceRegistration> registrations)
        {
            registrations = this.serviceRepository.GetValueOrDefault(type.GetHashCode());
            return registrations != null;
        }

        private bool TryGetByNamedKey(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            ImmutableTree<IServiceRegistration> registrations;
            if (this.TryGetRegistrationsByType(typeInfo.Type, out registrations))
            {
                registration = registrations.GetValueOrDefault(typeInfo.DependencyName.GetHashCode());
                return registration != null;
            }

            registration = null;
            return false;
        }

        private bool TryGetByNamedKeyWithoutGenericDefinitionExtraction(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            ImmutableTree<IServiceRegistration> registrations;
            if (this.TryGetRegistrationsByTypeWithoutGenericDefinitionExtraction(typeInfo.Type, out registrations))
            {
                registration = registrations.GetValueOrDefault(typeInfo.DependencyName.GetHashCode());
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
