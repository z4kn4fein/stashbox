using Stashbox.BuildUp;
using Stashbox.ContainerExtension;
using Stashbox.Lifetime;
using Stashbox.Utils;
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
                this.containerContext.Container.RootScope.InvalidateDelegateCache();
            }
            else
                this.containerContext.RegistrationRepository.AddOrUpdateRegistration(registration, false, replace);

            if (replace)
                this.containerContext.Container.RootScope.InvalidateDelegateCache();

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

            this.containerContext.Container.RootScope.InvalidateDelegateCache();

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.containerContext, registration);

            return this.containerContext.Container;
        }

        /// <inheritdoc />
        public IServiceRegistration CreateServiceRegistration(IRegistrationContextMeta registrationContextMeta, bool isDecorator)
        {
            this.PreProcessExistingInstanceIfNeeded(registrationContextMeta);

            registrationContextMeta.Context.Lifetime = this.ChooseLifeTime(registrationContextMeta);

            var objectBuilder = this.CreateObjectBuilder(registrationContextMeta);

            var shouldHandleDisposal = this.ShouldHandleDisposal(registrationContextMeta);

            return this.ProduceServiceRegistration(objectBuilder, registrationContextMeta, isDecorator, shouldHandleDisposal);
        }

        private void PreProcessExistingInstanceIfNeeded(IRegistrationContextMeta meta)
        {
            if (meta.Context.ExistingInstance == null) return;

            if (!meta.Context.IsLifetimeExternallyOwned && meta.Context.ExistingInstance is IDisposable disposable)
                this.containerContext.Container.RootScope.AddDisposableTracking(disposable);

            if (meta.Context.Finalizer == null) return;

            var method = Constants.AddWithFinalizerMethod.MakeGenericMethod(meta.ServiceType);
            method.Invoke(this.containerContext.Container.RootScope, new[] { meta.Context.ExistingInstance, meta.Context.Finalizer });
        }

        private bool ShouldHandleDisposal(IRegistrationContextMeta meta)
        {
            if (meta.Context.IsLifetimeExternallyOwned)
                return false;

            if (meta.Context.ExistingInstance != null)
                return false;

            if (meta.Context.Lifetime == null && this.containerContext.ContainerConfigurator.ContainerConfiguration.TrackTransientsForDisposalEnabled)
                return true;

            return meta.Context.Lifetime != null;
        }

        private IObjectBuilder CreateObjectBuilder(IRegistrationContextMeta meta)
        {
            if (meta.ImplementationType.IsOpenGenericType())
                return this.objectBuilderSelector.Get(ObjectBuilder.Generic);

            if (meta.Context.ExistingInstance != null)
                return meta.Context.IsWireUp
                    ? this.objectBuilderSelector.Get(ObjectBuilder.WireUp)
                    : this.objectBuilderSelector.Get(ObjectBuilder.Instance);

            return meta.Context.ContainerFactory != null
                ? this.objectBuilderSelector.Get(ObjectBuilder.Factory)
                : this.objectBuilderSelector.Get(meta.Context.SingleFactory != null
                    ? ObjectBuilder.Factory
                    : ObjectBuilder.Default);
        }

        private IServiceRegistration ProduceServiceRegistration(IObjectBuilder objectBuilder, IRegistrationContextMeta meta, bool isDecorator, bool shouldHandleDisposal) =>
            new ServiceRegistration(meta.ServiceType, meta.ImplementationType, this.containerContext.ContainerConfigurator,
                objectBuilder, meta.Context, isDecorator, shouldHandleDisposal);


        private ILifetime ChooseLifeTime(IRegistrationContextMeta meta) => meta.Context.ExistingInstance != null
            ? meta.Context.IsWireUp
                ? new SingletonLifetime()
                : null
            : meta.Context.Lifetime;
    }
}
