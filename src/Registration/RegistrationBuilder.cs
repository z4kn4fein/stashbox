using Stashbox.BuildUp;
using Stashbox.Lifetime;
using Stashbox.Utils;
using System;

namespace Stashbox.Registration
{
    internal class RegistrationBuilder : IRegistrationBuilder
    {
        private readonly IContainerContext containerContext;
        private readonly IObjectBuilderSelector objectBuilderSelector;

        public RegistrationBuilder(IContainerContext containerContext, IObjectBuilderSelector objectBuilderSelector)
        {
            this.containerContext = containerContext;
            this.objectBuilderSelector = objectBuilderSelector;
        }

        public IServiceRegistration BuildServiceRegistration(IRegistrationContext registrationContext, bool isDecorator)
        {
            this.PreProcessExistingInstanceIfNeeded(registrationContext);
            registrationContext.Context.Lifetime = this.ChooseLifeTime(registrationContext);

            var shouldHandleDisposal = this.ShouldHandleDisposal(registrationContext);
            return new ServiceRegistration(registrationContext.ImplementationType, this.containerContext.ContainerConfigurator,
                this.objectBuilderSelector, registrationContext.Context, isDecorator, shouldHandleDisposal);
        }

        private void PreProcessExistingInstanceIfNeeded(IRegistrationContext registrationContext)
        {
            if (registrationContext.Context.ExistingInstance == null) return;

            if (!registrationContext.Context.IsLifetimeExternallyOwned && registrationContext.Context.ExistingInstance is IDisposable disposable)
                this.containerContext.Container.RootScope.AddDisposableTracking(disposable);

            if (registrationContext.Context.Finalizer == null) return;

            var method = Constants.AddWithFinalizerMethod.MakeGenericMethod(registrationContext.ServiceType);
            method.Invoke(this.containerContext.Container.RootScope, new[] { registrationContext.Context.ExistingInstance, registrationContext.Context.Finalizer });
        }

        private bool ShouldHandleDisposal(IRegistrationContext registrationContext)
        {
            if (registrationContext.Context.IsLifetimeExternallyOwned)
                return false;

            if (registrationContext.Context.ExistingInstance != null)
                return false;

            if (registrationContext.Context.Lifetime == null && this.containerContext.ContainerConfigurator.ContainerConfiguration.TrackTransientsForDisposalEnabled)
                return true;

            return registrationContext.Context.Lifetime != null;
        }

        private ILifetime ChooseLifeTime(IRegistrationContext registrationContext) => registrationContext.Context.ExistingInstance != null
            ? registrationContext.Context.IsWireUp
                ? new SingletonLifetime()
                : null
            : registrationContext.Context.Lifetime;
    }
}
