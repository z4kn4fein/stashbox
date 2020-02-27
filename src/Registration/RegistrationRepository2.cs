using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Registration.Extensions;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.Registration
{
    internal class RegistrationRepository2 : IRegistrationRepository
    {
        private readonly ContainerConfiguration containerConfiguration;
        private AvlTreeKeyValue<Type, ArrayStoreKeyed<object, IServiceRegistration>> serviceRepository = AvlTreeKeyValue<Type, ArrayStoreKeyed<object, IServiceRegistration>>.Empty;

        public RegistrationRepository2(ContainerConfiguration containerConfiguration)
        {
            this.containerConfiguration = containerConfiguration;
        }

        public void AddOrUpdateRegistration(IServiceRegistration registration, Type serviceType, bool remap, bool replace)
        {
            var newRepository = new ArrayStoreKeyed<object, IServiceRegistration>(registration.RegistrationId, registration);

            if (remap)
                Swap.SwapValue(ref this.serviceRepository, (t1, t2, t3, t4, repo) =>
                    repo.AddOrUpdate(t1, t2, true), serviceType, newRepository, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            else
                Swap.SwapValue(ref this.serviceRepository, (t1, t2, t3, t4, repo) =>
                    repo.AddOrUpdate(t2, t3,
                        (oldValue, newValue) => oldValue.AddOrUpdate(t1.RegistrationId, t1, t4)),
                        registration, serviceType, newRepository, replace);
        }

        public bool ContainsRegistration(Type type, object name) =>
            this.serviceRepository.ContainsRegistration(type, name);

        public IEnumerable<KeyValuePair<Type, IServiceRegistration>> GetRegistrationMappings() =>
             this.serviceRepository.Walk().SelectMany(reg => reg.Value.Select(r => new KeyValuePair<Type, IServiceRegistration>(reg.Key, r)));

        public IServiceRegistration GetRegistrationOrDefault(Type type, ResolutionContext resolutionContext, object name = null)
        {
            throw new NotImplementedException();
        }

        public IServiceRegistration GetRegistrationOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValue<object, IServiceRegistration>> GetRegistrationsOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            throw new NotImplementedException();
        }

        private ArrayStoreKeyed<object, IServiceRegistration> GetRegistrationsForType(Type type)
        {
            var registrations = this.serviceRepository.GetOrDefault(type);
            if (registrations == null && type.IsClosedGenericType())
                return this.serviceRepository.GetOrDefault(type.GetGenericTypeDefinition());

            return registrations;
        }
    }
}
