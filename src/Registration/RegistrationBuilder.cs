using Stashbox.BuildUp;
using Stashbox.Configuration;
using Stashbox.Lifetime;
using Stashbox.Registration.Fluent;
using Stashbox.Utils;
using System;

namespace Stashbox.Registration
{
    internal class RegistrationBuilder : IRegistrationBuilder
    {
        private readonly ContainerConfiguration containerConfiguration;
        private readonly IResolutionScope rootScope;
        private readonly IObjectBuilderSelector objectBuilderSelector;

        public RegistrationBuilder(ContainerConfiguration containerConfiguration, IResolutionScope rootScope, IObjectBuilderSelector objectBuilderSelector)
        {
            this.containerConfiguration = containerConfiguration;
            this.rootScope = rootScope;
            this.objectBuilderSelector = objectBuilderSelector;
        }

        public IServiceRegistration BuildServiceRegistration(RegistrationConfiguration registrationConfiguration, bool isDecorator)
        {
            this.PreProcessExistingInstanceIfNeeded(registrationConfiguration.Context, registrationConfiguration.ImplementationType);
            registrationConfiguration.Context.Lifetime = this.ChooseLifeTime(registrationConfiguration.Context);

            var shouldHandleDisposal = this.ShouldHandleDisposal(registrationConfiguration.Context);

            return new ServiceRegistration(registrationConfiguration.ImplementationType, this.containerConfiguration,
                this.SelectObjectBuilder(registrationConfiguration.Context, registrationConfiguration.ImplementationType),
                registrationConfiguration.Context, isDecorator, shouldHandleDisposal);
        }

        private void PreProcessExistingInstanceIfNeeded(RegistrationContext registrationContext, Type implementationType)
        {
            if (registrationContext.ExistingInstance == null) return;

            if (!registrationContext.IsLifetimeExternallyOwned && registrationContext.ExistingInstance is IDisposable disposable)
                this.rootScope.AddDisposableTracking(disposable);

            if (registrationContext.Finalizer == null) return;

            var method = Constants.AddWithFinalizerMethod.MakeGenericMethod(implementationType);
            method.Invoke(this.rootScope, new[] { registrationContext.ExistingInstance, registrationContext.Finalizer });
        }

        private bool ShouldHandleDisposal(RegistrationContext registrationContext)
        {
            if (registrationContext.IsLifetimeExternallyOwned)
                return false;

            if (registrationContext.ExistingInstance != null)
                return false;

            if (registrationContext.Lifetime == null && this.containerConfiguration.TrackTransientsForDisposalEnabled)
                return true;

            return registrationContext.Lifetime != null;
        }

        private ILifetime ChooseLifeTime(RegistrationContext registrationContext) => registrationContext.ExistingInstance != null
            ? registrationContext.IsWireUp
                ? new SingletonLifetime()
                : null
            : registrationContext.Lifetime ?? this.containerConfiguration.DefaultLifetime?.Create();

        private IObjectBuilder SelectObjectBuilder(RegistrationContext registrationContext, Type implementationType)
        {
            if (implementationType.IsOpenGenericType())
                return this.objectBuilderSelector.Get(ObjectBuilder.Generic);

            if (registrationContext.ExistingInstance != null)
                return registrationContext.IsWireUp
                    ? this.objectBuilderSelector.Get(ObjectBuilder.WireUp)
                    : this.objectBuilderSelector.Get(ObjectBuilder.Instance);

            return registrationContext.ContainerFactory != null
                ? this.objectBuilderSelector.Get(ObjectBuilder.Factory)
                : this.objectBuilderSelector.Get(registrationContext.SingleFactory != null
                    ? ObjectBuilder.Factory
                    : ObjectBuilder.Default);
        }
    }
}
