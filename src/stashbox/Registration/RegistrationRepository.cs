using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq;

namespace Stashbox.Registration
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly Ref<ImmutableTree<Type, Ref<ImmutableTree<string, IServiceRegistration>>>> serviceRepository;
        private readonly object syncObject = new object();

        public RegistrationRepository()
        {
            this.serviceRepository = new Ref<ImmutableTree<Type, Ref<ImmutableTree<string, IServiceRegistration>>>>(ImmutableTree<Type, Ref<ImmutableTree<string, IServiceRegistration>>>.Empty);
        }

        public bool TryGetRegistrationWithConditions(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            Shield.EnsureNotNull(typeInfo);

            return typeInfo.DependencyName == null ? this.TryGetByTypeKeyWithConditions(typeInfo, out registration) : this.TryGetByNamedKey(typeInfo, out registration);
        }

        public bool TryGetRegistration(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            Shield.EnsureNotNull(typeInfo);

            return typeInfo.DependencyName == null ? this.TryGetByTypeKey(typeInfo, out registration) : this.TryGetByNamedKey(typeInfo, out registration);
        }

        public bool TryGetAllRegistrations(TypeInformation typeInfo, out IServiceRegistration[] registrations)
        {
            Shield.EnsureNotNull(typeInfo);
            return this.TryGetAllByTypedKey(typeInfo, out registrations);
        }

        public void AddRegistration(Type typeKey, IServiceRegistration registration, string nameKey)
        {
            Shield.EnsureNotNull(typeKey);
            Shield.EnsureNotNull(registration);

            var immutableTree = new Ref<ImmutableTree<string, IServiceRegistration>>(ImmutableTree<string, IServiceRegistration>.Empty);
            var newTree = new Ref<ImmutableTree<string, IServiceRegistration>>(immutableTree.Value.AddOrUpdate(nameKey, registration));

            lock (this.syncObject)
            {
                var newRepository = this.serviceRepository.Value.AddOrUpdate(typeKey, newTree, (oldValue, newValue) =>
                {
                    var newRegistration = oldValue.Value.AddOrUpdate(nameKey, registration, (oldRegistration, newReg) => newReg);

                    if (!oldValue.TrySwapIfStillCurrent(oldValue.Value, newRegistration))
                        oldValue.Swap(_ => newRegistration);

                    return oldValue;
                });

                if (!this.serviceRepository.TrySwapIfStillCurrent(this.serviceRepository.Value, newRepository))
                    this.serviceRepository.Swap(_ => newRepository);
            }
        }

        public bool TryGetTypedRepositoryRegistrations(TypeInformation typeInfo, out IServiceRegistration[] registrations)
        {
            var serviceRegistrations = this.serviceRepository.Value.GetValueOrDefault(typeInfo.Type);
            if (serviceRegistrations == null)
            {
                Type genericTypeDefinition;
                if (this.TryHandleOpenGenericType(typeInfo.Type, out genericTypeDefinition))
                {
                    serviceRegistrations = this.serviceRepository.Value.GetValueOrDefault(genericTypeDefinition);
                }
                else
                {
                    registrations = null;
                    return false;
                }
            }

            registrations = serviceRegistrations?.Value?.Enumerate().Select(reg => reg.Value).ToArray();
            return registrations != null;
        }

        public bool ConstainsTypeKey(TypeInformation typeInfo)
        {
            return this.serviceRepository.Value.GetValueOrDefault(typeInfo.Type) != null;
        }

        public bool ConstainsTypeKeyWithConditions(TypeInformation typeInfo)
        {
            var registrations = this.serviceRepository.Value.GetValueOrDefault(typeInfo.Type);
            if (registrations == null)
            {
                Type genericTypeDefinition;
                if (this.TryHandleOpenGenericType(typeInfo.Type, out genericTypeDefinition))
                {
                    registrations = this.serviceRepository.Value.GetValueOrDefault(genericTypeDefinition);
                    return registrations != null && registrations.Value != null && registrations.Value.Enumerate().Any(registration => registration.Value.IsUsableForCurrentContext(new TypeInformation
                    {
                        Type = genericTypeDefinition,
                        ParentType = typeInfo.ParentType,
                        DependencyName = typeInfo.DependencyName,
                        CustomAttributes = typeInfo.CustomAttributes
                    }));
                }
                else
                    return false;
            }
            else
                return registrations.Value != null && registrations.Value.Enumerate().Any(registration => registration.Value.IsUsableForCurrentContext(typeInfo));
        }

        public bool ConstainsTypeKeyWithConditionsWithoutGenericDefinitionExtraction(TypeInformation typeInfo)
        {
            var registrations = this.serviceRepository.Value.GetValueOrDefault(typeInfo.Type);
            if (registrations == null) return false;
            return registrations.Value != null && registrations.Value.Enumerate().Any(registration => registration.Value.IsUsableForCurrentContext(typeInfo));
        }

        public void CleanUp()
        {
            foreach (var registration in this.serviceRepository.Value.Enumerate().Select(reg => reg.Value).SelectMany(registrations => registrations.Value.Enumerate()))
            {
                registration.Value.CleanUp();
            }

            this.serviceRepository.Swap(_ => null);
        }

        private bool TryGetByTypeKey(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            ImmutableTree<string, IServiceRegistration> registrations;
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
            ImmutableTree<string, IServiceRegistration> registrations;
            if (!this.TryGetRegistrationsByType(typeInfo.Type, out registrations))
            {
                registration = null;
                return false;
            }

            var enumeratedRegistrations = registrations.Enumerate().Select(reg => reg.Value);

            if (enumeratedRegistrations.Any(reg => reg.HasCondition))
                registration = enumeratedRegistrations.Where(reg => reg.HasCondition)
                                                   .FirstOrDefault(reg => reg.IsUsableForCurrentContext(typeInfo));
            else
                registration = enumeratedRegistrations.FirstOrDefault(reg => reg.IsUsableForCurrentContext(typeInfo));

            return registration != null;
        }

        private bool TryGetRegistrationsByType(Type type, out ImmutableTree<string, IServiceRegistration> registrations)
        {
            var serviceRegistrations = this.serviceRepository.Value.GetValueOrDefault(type);
            if (serviceRegistrations == null)
            {
                Type genericTypeDefinition;
                if (this.TryHandleOpenGenericType(type, out genericTypeDefinition))
                {
                    serviceRegistrations = this.serviceRepository.Value.GetValueOrDefault(genericTypeDefinition);
                }
                else
                {
                    registrations = null;
                    return false;
                }
            }

            registrations = serviceRegistrations?.Value;
            return registrations != null;
        }

        private bool TryGetAllByTypedKey(TypeInformation typeInfo, out IServiceRegistration[] registrations)
        {
            return this.TryGetTypedRepositoryRegistrations(typeInfo, out registrations);
        }

        private bool TryGetByNamedKey(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            ImmutableTree<string, IServiceRegistration> registrations;
            if (this.TryGetRegistrationsByType(typeInfo.Type, out registrations))
            {
                registration = registrations.GetValueOrDefault(typeInfo.DependencyName);
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
