using Stashbox.ContainerExtension;
using System;

namespace Stashbox.Registration
{
    internal class ServiceRegistrator : IServiceRegistrator
    {
        private readonly IContainerContext containerContext;
        private readonly IContainerExtensionManager containerExtensionManager;

        internal ServiceRegistrator(IContainerContext containerContext, IContainerExtensionManager containerExtensionManager)
        {
            this.containerContext = containerContext;
            this.containerExtensionManager = containerExtensionManager;
        }
        
        public IRegistrationContext PrepareContext(Type serviceType, Type implementationType) =>
            new RegistrationContext(serviceType, implementationType);
        
        public IRegistrationContext<TService> PrepareContext<TService>(Type serviceType, Type implementationType) =>
            new RegistrationContext<TService>(serviceType, implementationType);
        
        public IRegistrationContext PrepareContext(Type serviceType, Type implementationType,
             RegistrationContextData registrationContextData) =>
            new RegistrationContext(serviceType, implementationType, registrationContextData);
        
        public IDecoratorRegistrationContext PrepareDecoratorContext(Type serviceType, Type implementationType) =>
            new DecoratorRegistrationContext(new RegistrationContext(serviceType, implementationType));
        
        public IStashboxContainer Register(IServiceRegistration serviceRegistration, bool replace)
        {
            if (serviceRegistration.IsDecorator)
            {
                this.containerContext.DecoratorRepository.AddDecorator(serviceRegistration.ServiceType, serviceRegistration,
                    false, replace);
                this.containerContext.Container.RootScope.InvalidateDelegateCache();
            }
            else
                this.containerContext.RegistrationRepository.AddOrUpdateRegistration(serviceRegistration, false, replace);

            if (replace)
                this.containerContext.Container.RootScope.InvalidateDelegateCache();

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.containerContext, serviceRegistration);

            return this.containerContext.Container;
        }
        
        public IStashboxContainer ReMap(IServiceRegistration serviceRegistration)
        {
            if (serviceRegistration.IsDecorator)
                this.containerContext.DecoratorRepository.AddDecorator(serviceRegistration.ServiceType, serviceRegistration,
                    true, false);
            else
                this.containerContext.RegistrationRepository.AddOrUpdateRegistration(serviceRegistration, true, false);

            this.containerContext.Container.RootScope.InvalidateDelegateCache();

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.containerContext, serviceRegistration);

            return this.containerContext.Container;
        }
    }
}
