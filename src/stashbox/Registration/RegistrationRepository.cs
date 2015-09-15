using Ronin.Common;
using Stashbox.Infrastructure;
using System;
using System.Collections.Generic;
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

        public bool TryGetRegistration(Type typeKey, out IServiceRegistration registration, string nameKey = null)
        {
            Shield.EnsureNotNull(typeKey);

            return nameKey == null ? this.TryGetByTypeKey(typeKey, out registration) : this.TryGetByNamedKey(typeKey, nameKey, out registration);
        }

        public bool TryGetAllRegistrations(Type typeKey, out IEnumerable<IServiceRegistration> registrations)
        {
            Shield.EnsureNotNull(typeKey);

            return this.TryGetAllByTypedKey(typeKey, out registrations);
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

        public bool TryGetTypedRepositoryRegistrations(Type typeKey, out IDictionary<string, IServiceRegistration> registrations)
        {
            using (this.readerWriterLockSlim.AquireReadLock())
            {
                Type genericTypeDefinition;
                return this.serviceRepository.TryGetValue(typeKey, out registrations) ||
                    (this.TryHandleOpenGenericType(typeKey, out genericTypeDefinition) &&
                    this.serviceRepository.TryGetValue(genericTypeDefinition, out registrations));
            }
        }

        public bool ConstainsTypeKey(Type typeKey)
        {
            using (this.readerWriterLockSlim.AquireReadLock())
            {
                Type genericTypeDefinition;
                return this.serviceRepository.ContainsKey(typeKey) ||
                    (this.TryHandleOpenGenericType(typeKey, out genericTypeDefinition) && this.serviceRepository.ContainsKey(genericTypeDefinition));
            }
        }

        public bool ConstainsTypeKeyWithoutGenericDefinitionExtraction(Type typeKey)
        {
            using (this.readerWriterLockSlim.AquireReadLock())
            {
                return this.serviceRepository.ContainsKey(typeKey);
            }
        }

        private bool TryGetByTypeKey(Type typeKey, out IServiceRegistration registration)
        {
            using (this.readerWriterLockSlim.AquireReadLock())
            {
                IDictionary<string, IServiceRegistration> registrations;
                if (!this.serviceRepository.TryGetValue(typeKey, out registrations))
                {
                    Type genericTypeDefinition;
                    if (this.TryHandleOpenGenericType(typeKey, out genericTypeDefinition))
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
                var enumerator = registrations.GetEnumerator();
                enumerator.MoveNext();
                registration = enumerator.Current.Value;
                return true;
            }
        }

        private bool TryGetAllByTypedKey(Type typeKey, out IEnumerable<IServiceRegistration> registrations)
        {
            using (this.readerWriterLockSlim.AquireReadLock())
            {
                IDictionary<string, IServiceRegistration> typedRegistrations = null;
                if (this.TryGetTypedRepositoryRegistrations(typeKey, out typedRegistrations))
                {
                    registrations = typedRegistrations.Values;
                    return true;
                }

                registrations = null;
                return false;
            }
        }

        private bool TryGetByNamedKey(Type typeKey, string nameKey, out IServiceRegistration registration)
        {
            using (this.readerWriterLockSlim.AquireReadLock())
            {
                IDictionary<string, IServiceRegistration> registrations = null;
                if (this.TryGetTypedRepositoryRegistrations(typeKey, out registrations))
                {
                    registration = registrations[nameKey];
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
