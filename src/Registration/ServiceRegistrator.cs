using Stashbox.ContainerExtension;
using System;
using System.Linq;

namespace Stashbox.Registration
{
    internal class ServiceRegistrator : IServiceRegistrator
    {
        private readonly IContainerExtensionManager containerExtensionManager;

        internal ServiceRegistrator(IContainerExtensionManager containerExtensionManager)
        {
            this.containerExtensionManager = containerExtensionManager;
        }

        public void Register(IContainerContext containerContext, IServiceRegistration serviceRegistration, Type serviceType, RegistrationContext registrationContext)
        {
            if (serviceRegistration.IsDecorator)
                this.Register(containerContext, serviceRegistration, serviceType, registrationContext.ReplaceExistingRegistration);
            else if (registrationContext.AdditionalServiceTypes.Any())
                foreach (var additionalServiceType in registrationContext.AdditionalServiceTypes)
                    this.Register(containerContext, serviceRegistration, additionalServiceType, registrationContext.ReplaceExistingRegistration);

            this.Register(containerContext, serviceRegistration, serviceType, registrationContext.ReplaceExistingRegistration);
        }

        public void Register(IContainerContext containerContext, IServiceRegistration serviceRegistration, Type serviceType, bool replace)
        {
            if (serviceRegistration.IsDecorator)
            {
                containerContext.DecoratorRepository.AddDecorator(serviceType, serviceRegistration, false, replace);
                containerContext.Container.RootScope.InvalidateDelegateCache();
            }
            else
                containerContext.RegistrationRepository.AddOrUpdateRegistration(serviceRegistration, serviceType, false, replace);

            if (replace)
                containerContext.Container.RootScope.InvalidateDelegateCache();

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(containerContext, serviceRegistration);
        }

        public void ReMap(IContainerContext containerContext, IServiceRegistration serviceRegistration, Type serviceType, RegistrationContext registrationContext)
        {
            if (serviceRegistration.IsDecorator)
                containerContext.DecoratorRepository.AddDecorator(serviceType, serviceRegistration, true, false);
            else
            {
                if (registrationContext.AdditionalServiceTypes.Any())
                    foreach (var additionalServiceType in registrationContext.AdditionalServiceTypes)
                        containerContext.RegistrationRepository.AddOrUpdateRegistration(serviceRegistration, additionalServiceType, true, false);

                containerContext.RegistrationRepository.AddOrUpdateRegistration(serviceRegistration, serviceType, true, false);
            }

            containerContext.Container.RootScope.InvalidateDelegateCache();

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(containerContext, serviceRegistration);
        }
    }
}
