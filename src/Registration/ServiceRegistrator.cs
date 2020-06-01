using System;
using System.Linq;

namespace Stashbox.Registration
{
    internal class ServiceRegistrator
    {
        public void Register(IContainerContext containerContext, ServiceRegistration serviceRegistration, Type serviceType)
        {
            if (serviceRegistration.IsDecorator)
                this.Register(containerContext, serviceRegistration, serviceType, serviceRegistration.RegistrationContext.ReplaceExistingRegistration);
            else if (serviceRegistration.RegistrationContext.AdditionalServiceTypes.Any())
                foreach (var additionalServiceType in serviceRegistration.RegistrationContext.AdditionalServiceTypes)
                    this.Register(containerContext, serviceRegistration, additionalServiceType, serviceRegistration.RegistrationContext.ReplaceExistingRegistration);

            this.Register(containerContext, serviceRegistration, serviceType, serviceRegistration.RegistrationContext.ReplaceExistingRegistration);
        }

        public void Register(IContainerContext containerContext, ServiceRegistration serviceRegistration, Type serviceType, bool replace)
        {
            if (serviceRegistration.IsDecorator)
            {
                containerContext.DecoratorRepository.AddDecorator(serviceType, serviceRegistration, false, replace);
                containerContext.RootScope.InvalidateDelegateCache();
            }
            else
                containerContext.RegistrationRepository.AddOrUpdateRegistration(serviceRegistration, serviceType, false, replace);

            if (replace)
                containerContext.RootScope.InvalidateDelegateCache();
        }

        public void ReMap(IContainerContext containerContext, ServiceRegistration serviceRegistration, Type serviceType)
        {
            if (serviceRegistration.IsDecorator)
                containerContext.DecoratorRepository.AddDecorator(serviceType, serviceRegistration, true, false);
            else
            {
                if (serviceRegistration.RegistrationContext.AdditionalServiceTypes.Any())
                    foreach (var additionalServiceType in serviceRegistration.RegistrationContext.AdditionalServiceTypes)
                        containerContext.RegistrationRepository.AddOrUpdateRegistration(serviceRegistration, additionalServiceType, true, false);

                containerContext.RegistrationRepository.AddOrUpdateRegistration(serviceRegistration, serviceType, true, false);
            }

            containerContext.RootScope.InvalidateDelegateCache();
        }
    }
}
