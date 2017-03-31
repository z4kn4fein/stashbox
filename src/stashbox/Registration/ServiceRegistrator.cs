using System;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Infrastructure.Registration;
using Stashbox.MetaInfo;

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
        public IRegistrationContext PrepareContext(Type serviceType, Type implementationType,
             RegistrationContextData registrationContextData) =>
            new RegistrationContext(serviceType, implementationType, this, registrationContextData);

        /// <inheritdoc />
        public IDecoratorRegistrationContext PrepareDecoratorContext(Type serviceType, Type implementationType) =>
            new DecoratorRegistrationContext(new RegistrationContext(serviceType, implementationType, this), this);

        /// <inheritdoc />
        public IStashboxContainer Register(IRegistrationContextMeta registrationContextMeta, bool isDecorator)
        {
            var registration = this.CreateServiceRegistration(registrationContextMeta, isDecorator);

            if (isDecorator)
            {
                this.containerContext.DecoratorRepository.AddDecorator(registrationContextMeta.ServiceType, registration);
                this.containerContext.DelegateRepository.InvalidateDelegateCache();
            }
            else
                this.containerContext.RegistrationRepository.AddOrUpdateRegistration(registrationContextMeta.ServiceType, registrationContextMeta.Context.Name, false, registration);

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.containerContext, registrationContextMeta.ServiceType,
                registrationContextMeta.ImplementationType, registrationContextMeta.Context.InjectionParameters);
            
            return this.containerContext.Container;
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap(IRegistrationContextMeta registrationContextMeta)
        {
            var registration = this.CreateServiceRegistration(registrationContextMeta, false);

            this.containerContext.RegistrationRepository.AddOrUpdateRegistration(registrationContextMeta.ServiceType, registrationContextMeta.Context.Name, true, registration);

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.containerContext, registrationContextMeta.ServiceType,
                registrationContextMeta.ImplementationType, registrationContextMeta.Context.InjectionParameters);

            this.containerContext.DelegateRepository.InvalidateDelegateCache();

            return this.containerContext.Container;
        }

        /// <inheritdoc />
        public IServiceRegistration CreateServiceRegistration(IRegistrationContextMeta registrationContextMeta, bool isDecorator)
        {
            var registrationLifetime = this.ChooseLifeTime(registrationContextMeta);
            var metaInfoProvider = new MetaInfoProvider(this.containerContext, registrationContextMeta.Context, 
                registrationContextMeta.ImplementationType, registrationContextMeta.Context.InjectionParameters);

            var shouldHandleDisposal = this.ShouldHandleDisposal(registrationContextMeta, registrationLifetime);

            var objectBuilder = this.CreateObjectBuilder(registrationContextMeta);

            return this.ProduceServiceRegistration(registrationLifetime, objectBuilder, metaInfoProvider, 
                registrationContextMeta, isDecorator, shouldHandleDisposal);
        }

        private bool ShouldHandleDisposal(IRegistrationContextMeta meta, ILifetime registrationLifetime)
        {
            if (meta.Context.IsLifetimeExternallyOwned)
                return false;

            if (registrationLifetime == null && this.containerContext.ContainerConfigurator.ContainerConfiguration.TrackTransientsForDisposalEnabled)
                return true;

            return registrationLifetime != null;
        }

        private IObjectBuilder CreateObjectBuilder(IRegistrationContextMeta meta)
        {
            if (meta.ImplementationType.IsOpenGenericType())
                return this.objectBuilderSelector.Get(ObjectBuilder.Generic);

            if (meta.Context.ExistingInstance != null)
                return this.objectBuilderSelector.Get(ObjectBuilder.Instance);

            if (meta.Context.ContainerFactory != null)
                return this.objectBuilderSelector.Get(ObjectBuilder.Factory);

            if (meta.Context.SingleFactory != null)
                return this.objectBuilderSelector.Get(ObjectBuilder.Factory);

            return this.objectBuilderSelector.Get(ObjectBuilder.Default);
        }

        private IServiceRegistration ProduceServiceRegistration(ILifetime lifeTime, IObjectBuilder objectBuilder, IMetaInfoProvider metaInfoProvider, IRegistrationContextMeta meta, bool isDecorator, bool shouldHandleDisposal)
        {
            return new ServiceRegistration(meta.ServiceType, meta.ImplementationType, this.containerContext.ReserveRegistrationNumber(), 
                lifeTime, objectBuilder, metaInfoProvider, meta.Context, isDecorator, shouldHandleDisposal);
        }

        private ILifetime ChooseLifeTime(IRegistrationContextMeta meta) => meta.Context.ExistingInstance != null ? null :
            meta.Context.Lifetime;
    }
}
