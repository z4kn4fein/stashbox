using System;
using System.Linq;

namespace Stashbox.Registration
{
    internal class ServiceRegistrator
    {
        public void Register(IContainerContext containerContext, ServiceRegistration serviceRegistration, Type serviceType)
        {
            if (serviceRegistration.RegistrationContext.AdditionalServiceTypes.Any())
                foreach (var additionalServiceType in serviceRegistration.RegistrationContext.AdditionalServiceTypes)
                    this.RegisterInternal(containerContext, serviceRegistration, additionalServiceType);

            this.RegisterInternal(containerContext, serviceRegistration, serviceType);
        }

        private void RegisterInternal(IContainerContext containerContext, ServiceRegistration serviceRegistration, Type serviceType)
        {
            if (serviceRegistration.IsDecorator)
            {
                containerContext.DecoratorRepository.AddDecorator(serviceType, serviceRegistration, false);
                containerContext.RootScope.InvalidateDelegateCache();
            }
            else if (containerContext.RegistrationRepository.AddOrUpdateRegistration(serviceRegistration, serviceType))
                containerContext.RootScope.InvalidateDelegateCache();
        }

        public void ReMap(IContainerContext containerContext, ServiceRegistration serviceRegistration, Type serviceType)
        {
            if (serviceRegistration.RegistrationContext.AdditionalServiceTypes.Any())
                foreach (var additionalServiceType in serviceRegistration.RegistrationContext.AdditionalServiceTypes)
                    this.ReMapInternal(containerContext, serviceRegistration, additionalServiceType);

            this.ReMapInternal(containerContext, serviceRegistration, serviceType);
        }

        private void ReMapInternal(IContainerContext containerContext, ServiceRegistration serviceRegistration, Type serviceType)
        {
            if (serviceRegistration.IsDecorator)
                containerContext.DecoratorRepository.AddDecorator(serviceType, serviceRegistration, true);
            else
                containerContext.RegistrationRepository.AddOrReMapRegistration(serviceRegistration, serviceType);

            containerContext.RootScope.InvalidateDelegateCache();
        }
    }
}
