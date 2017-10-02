using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Infrastructure.Registration;
using System;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a service registrator.
    /// </summary>
    public class ServiceRegistrator : IServiceRegistrator
    {
        private readonly IContainerContext containerContext;
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IObjectBuilderSelector objectBuilderSelector;

        internal ServiceRegistrator(IContainerContext containerContext, IContainerExtensionManager containerExtensionManager,
            IObjectBuilderSelector objectBuilderSelector)
        {
            this.containerContext = containerContext;
            this.containerExtensionManager = containerExtensionManager;
            this.objectBuilderSelector = objectBuilderSelector;
        }

        /// <inheritdoc />
        public IRegistrationContext PrepareContext(Type serviceType, Type implementationType) =>
            new RegistrationContext(serviceType, implementationType, this);

        /// <inheritdoc />
        public IRegistrationContext<TService> PrepareContext<TService>(Type serviceType, Type implementationType) =>
            new RegistrationContext<TService>(serviceType, implementationType, this);

        /// <inheritdoc />
        public IRegistrationContext PrepareContext(Type serviceType, Type implementationType,
             RegistrationContextData registrationContextData) =>
            new RegistrationContext(serviceType, implementationType, this, registrationContextData);

        /// <inheritdoc />
        public IDecoratorRegistrationContext PrepareDecoratorContext(Type serviceType, Type implementationType) =>
            new DecoratorRegistrationContext(new RegistrationContext(serviceType, implementationType, this), this);

        /// <inheritdoc />
        public IStashboxContainer Register(IRegistrationContextMeta registrationContextMeta, bool isDecorator, bool replace)
        {
            var registration = this.CreateServiceRegistration(registrationContextMeta, isDecorator);

            if (isDecorator)
            {
                this.containerContext.DecoratorRepository.AddDecorator(registrationContextMeta.ServiceType, registration,
                    false, replace);
                this.containerContext.DelegateRepository.InvalidateDelegateCache();
            }
            else
                this.containerContext.RegistrationRepository.AddOrUpdateRegistration(registration, false, replace);

            if (replace)
                this.containerContext.DelegateRepository.InvalidateDelegateCache();

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.containerContext, registration);

            return this.containerContext.Container;
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap(IRegistrationContextMeta registrationContextMeta, bool isDecorator)
        {
            var registration = this.CreateServiceRegistration(registrationContextMeta, isDecorator);

            if (isDecorator)
                this.containerContext.DecoratorRepository.AddDecorator(registrationContextMeta.ServiceType, registration,
                    true, false);
            else
                this.containerContext.RegistrationRepository.AddOrUpdateRegistration(registration, true, false);

            this.containerContext.DelegateRepository.InvalidateDelegateCache();

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.containerContext, registration);

            return this.containerContext.Container;
        }

        /// <inheritdoc />
        public IServiceRegistration CreateServiceRegistration(IRegistrationContextMeta registrationContextMeta, bool isDecorator)
        {
            registrationContextMeta.Context.Lifetime = this.ChooseLifeTime(registrationContextMeta);

            var objectBuilder = this.CreateObjectBuilder(registrationContextMeta);

            var shouldHandleDisposal = this.ShouldHandleDisposal(registrationContextMeta, objectBuilder);

            return this.ProduceServiceRegistration(objectBuilder, registrationContextMeta, isDecorator, shouldHandleDisposal);
        }

        private bool ShouldHandleDisposal(IRegistrationContextMeta meta, IObjectBuilder objectBuilder)
        {
            if (meta.Context.IsLifetimeExternallyOwned)
                return false;

            if (meta.Context.Lifetime == null && this.containerContext.ContainerConfigurator.ContainerConfiguration.TrackTransientsForDisposalEnabled)
                return true;

            return meta.Context.Lifetime != null || objectBuilder.HandlesObjectLifecycle;
        }

        private IObjectBuilder CreateObjectBuilder(IRegistrationContextMeta meta)
        {
            if (meta.ImplementationType.IsOpenGenericType())
                return this.objectBuilderSelector.Get(ObjectBuilder.Generic);

            if (meta.Context.ExistingInstance != null)
                return this.objectBuilderSelector.Get(ObjectBuilder.Instance);

            return meta.Context.ContainerFactory != null ?
                this.objectBuilderSelector.Get(ObjectBuilder.Factory) :
                    this.objectBuilderSelector.Get(meta.Context.SingleFactory != null ?
                        ObjectBuilder.Factory :
                            ObjectBuilder.Default);
        }

        private IServiceRegistration ProduceServiceRegistration(IObjectBuilder objectBuilder, IRegistrationContextMeta meta, bool isDecorator, bool shouldHandleDisposal)
        {
            return new ServiceRegistration(meta.ServiceType, meta.ImplementationType, this.containerContext.ContainerConfigurator,
                objectBuilder, meta.Context, isDecorator, shouldHandleDisposal);
        }

        private ILifetime ChooseLifeTime(IRegistrationContextMeta meta) => meta.Context.ExistingInstance != null ? null : meta.Context.Lifetime;
    }
}
