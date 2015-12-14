using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq;

namespace Stashbox.Registration
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private ImmutableTree<Type, ImmutableTree<IServiceRegistration>> serviceRepository;
        private readonly object syncObject = new object();

        public RegistrationRepository()
        {
            this.serviceRepository = ImmutableTree<Type, ImmutableTree<IServiceRegistration>>.Empty;
        }

        public bool TryGetRegistrationWithConditions(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            return typeInfo.DependencyName == null ? this.TryGetByTypeKeyWithConditions(typeInfo, out registration) : this.TryGetByNamedKey(typeInfo, out registration);
        }

        public bool TryGetRegistration(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            return typeInfo.DependencyName == null ? this.TryGetByTypeKey(typeInfo, out registration) : this.TryGetByNamedKey(typeInfo, out registration);
        }

        public bool TryGetAllRegistrations(TypeInformation typeInfo, out IServiceRegistration[] registrations)
        {
            return this.TryGetAllByTypedKey(typeInfo, out registrations);
        }

        public void AddRegistration(Type typeKey, IServiceRegistration registration, string nameKey)
        {
            var immutableTree = ImmutableTree<IServiceRegistration>.Empty;
            var newTree = immutableTree.AddOrUpdate(nameKey.GetHashCode(), registration);

            lock (this.syncObject)
            {
                this.serviceRepository = this.serviceRepository.AddOrUpdate(typeKey, newTree, (oldValue, newValue) =>
                {
                    return oldValue.AddOrUpdate(nameKey.GetHashCode(), registration, (oldRegistration, newReg) => newReg);
                });
            }
        }

        public bool TryGetTypedRepositoryRegistrations(TypeInformation typeInfo, out IServiceRegistration[] registrations)
        {
            var serviceRegistrations = this.serviceRepository.GetValueOrDefault(typeInfo.Type);
            if (serviceRegistrations == null)
            {
                Type genericTypeDefinition;
                if (this.TryHandleOpenGenericType(typeInfo.Type, out genericTypeDefinition))
                {
                    serviceRegistrations = this.serviceRepository.GetValueOrDefault(genericTypeDefinition);
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

        public bool ConstainsTypeKey(TypeInformation typeInfo)
        {
            return this.serviceRepository.GetValueOrDefault(typeInfo.Type) != null;
        }

        public bool ConstainsTypeKeyWithConditions(TypeInformation typeInfo)
        {
            var registrations = this.serviceRepository.GetValueOrDefault(typeInfo.Type);
            if (registrations != null)
                return registrations.Value != null &&
                       registrations.Enumerate()
                           .Any(registration => registration.Value.IsUsableForCurrentContext(typeInfo));
            {
                Type genericTypeDefinition;
                if (!this.TryHandleOpenGenericType(typeInfo.Type, out genericTypeDefinition)) return false;
                registrations = this.serviceRepository.GetValueOrDefault(genericTypeDefinition);
                return registrations != null && registrations.Enumerate().Any(registration => registration.Value.IsUsableForCurrentContext(new TypeInformation
                {
                    Type = genericTypeDefinition,
                    ParentType = typeInfo.ParentType,
                    DependencyName = typeInfo.DependencyName,
                    CustomAttributes = typeInfo.CustomAttributes
                }));
            }
        }

        public bool ConstainsTypeKeyWithConditionsWithoutGenericDefinitionExtraction(TypeInformation typeInfo)
        {
            var registrations = this.serviceRepository.GetValueOrDefault(typeInfo.Type);
            if (registrations == null) return false;
            return registrations.Value != null && registrations.Enumerate().Any(registration => registration.Value.IsUsableForCurrentContext(typeInfo));
        }

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

        private bool TryGetRegistrationsByType(Type type, out ImmutableTree<IServiceRegistration> registrations)
        {
            registrations = this.serviceRepository.GetValueOrDefault(type);
            if (registrations != null) return true;
            Type genericTypeDefinition;
            if (this.TryHandleOpenGenericType(type, out genericTypeDefinition))
            {
                registrations = this.serviceRepository.GetValueOrDefault(genericTypeDefinition);
            }
            else
            {
                return false;
            }

            return registrations != null;
        }

        private bool TryGetAllByTypedKey(TypeInformation typeInfo, out IServiceRegistration[] registrations)
        {
            return this.TryGetTypedRepositoryRegistrations(typeInfo, out registrations);
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
