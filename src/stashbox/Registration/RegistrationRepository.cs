using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Stashbox.Registration
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly IDictionary<Type, IDictionary<string, IServiceRegistration>> serviceRepository;
        private readonly DisposableReaderWriterLock readerWriterLockSlim;

        public RegistrationRepository()
        {
            this.serviceRepository = new Dictionary<Type, IDictionary<string, IServiceRegistration>>();
            this.readerWriterLockSlim = new DisposableReaderWriterLock(LockRecursionPolicy.SupportsRecursion);
        }

        public bool TryGetRegistration(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            Shield.EnsureNotNull(typeInfo);

            return typeInfo.DependencyName == null ? this.TryGetByTypeKey(typeInfo, out registration) : this.TryGetByNamedKey(typeInfo, out registration);
        }

        public bool TryGetAllRegistrations(TypeInformation typeInfo, out IEnumerable<IServiceRegistration> registrations)
        {
            Shield.EnsureNotNull(typeInfo);

            return this.TryGetAllByTypedKey(typeInfo, out registrations);
        }

        public void AddRegistration(Type typeKey, IServiceRegistration registration, string nameKey)
        {
            Shield.EnsureNotNull(typeKey);
            Shield.EnsureNotNull(registration);

            using (this.readerWriterLockSlim.AquireWriteLock())
            {
                IDictionary<string, IServiceRegistration> registrations;
                if (this.serviceRepository.TryGetValue(typeKey, out registrations))
                {
                    registrations.Add(nameKey, registration);
                }
                else
                {
                    var repository = new Dictionary<string, IServiceRegistration> { { nameKey, registration } };
                    this.serviceRepository.Add(typeKey, repository);
                }
            }
        }

        public bool TryGetTypedRepositoryRegistrations(TypeInformation typeInfo, out IDictionary<string, IServiceRegistration> registrations)
        {
            using (this.readerWriterLockSlim.AquireReadLock())
            {
                Type genericTypeDefinition;
                return this.serviceRepository.TryGetValue(typeInfo.Type, out registrations) ||
                    (this.TryHandleOpenGenericType(typeInfo.Type, out genericTypeDefinition) &&
                    this.serviceRepository.TryGetValue(genericTypeDefinition, out registrations));
            }
        }

        public bool ConstainsTypeKey(TypeInformation typeInfo)
        {
            using (this.readerWriterLockSlim.AquireReadLock())
            {
                IDictionary<string, IServiceRegistration> registrations;
                Type genericTypeDefinition;
                return this.TryGetTypedRepositoryRegistrations(typeInfo, out registrations) &&
                    registrations.Any(registration => registration.Value.IsUsableForCurrentContext(typeInfo)) ||
                    (this.TryHandleOpenGenericType(typeInfo.Type, out genericTypeDefinition) && this.TryGetTypedRepositoryRegistrations(typeInfo, out registrations) &&
                    registrations.Any(registration => registration.Value.IsUsableForCurrentContext(new TypeInformation
                    {
                        Type = genericTypeDefinition,
                        ParentType = typeInfo.ParentType,
                        DependencyName = typeInfo.DependencyName,
                        CustomAttributes = typeInfo.CustomAttributes
                    })));
            }
        }

        public bool ConstainsTypeKeyWithoutGenericDefinitionExtraction(TypeInformation typeInfo)
        {
            using (this.readerWriterLockSlim.AquireReadLock())
            {
                IDictionary<string, IServiceRegistration> registrations;
                return this.serviceRepository.TryGetValue(typeInfo.Type, out registrations) &&
                    registrations.Any(registration => registration.Value.IsUsableForCurrentContext(typeInfo));
            }
        }

        public void CleanUp()
        {
            using (this.readerWriterLockSlim.AquireWriteLock())
            {
                foreach (var registration in this.serviceRepository.SelectMany(registrations => registrations.Value))
                {
                    registration.Value.CleanUp();
                }

                this.serviceRepository.Clear();
            }
        }

        private bool TryGetByTypeKey(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            using (this.readerWriterLockSlim.AquireReadLock())
            {
                IDictionary<string, IServiceRegistration> registrations;
                if (!this.serviceRepository.TryGetValue(typeInfo.Type, out registrations))
                {
                    Type genericTypeDefinition;
                    if (this.TryHandleOpenGenericType(typeInfo.Type, out genericTypeDefinition))
                    {
                        if (!this.serviceRepository.TryGetValue(genericTypeDefinition, out registrations))
                        {
                            registration = null;
                            return false;
                        }
                    }
                    else
                    {
                        registration = null;
                        return false;
                    }
                }

                if (registrations.Values.Any(reg => reg.HasCondition))
                    registration = registrations.Values.Where(reg => reg.HasCondition)
                                                       .FirstOrDefault(reg => reg.IsUsableForCurrentContext(typeInfo));
                else
                    registration = registrations.Values.FirstOrDefault(reg => reg.IsUsableForCurrentContext(typeInfo));

                return registration != null;
            }
        }

        private bool TryGetAllByTypedKey(TypeInformation typeInfo, out IEnumerable<IServiceRegistration> registrations)
        {
            using (this.readerWriterLockSlim.AquireReadLock())
            {
                IDictionary<string, IServiceRegistration> typedRegistrations;
                if (this.TryGetTypedRepositoryRegistrations(typeInfo, out typedRegistrations))
                {
                    registrations = typedRegistrations.Values;
                    return true;
                }

                registrations = null;
                return false;
            }
        }

        private bool TryGetByNamedKey(TypeInformation typeInfo, out IServiceRegistration registration)
        {
            using (this.readerWriterLockSlim.AquireReadLock())
            {
                IDictionary<string, IServiceRegistration> registrations;
                if (this.TryGetTypedRepositoryRegistrations(typeInfo, out registrations))
                {
                    registration = registrations[typeInfo.DependencyName];
                    return true;
                }

                registration = null;
                return false;
            }
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
