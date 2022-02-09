using Stashbox.Lifetime;
using Stashbox.Registration.Fluent;
using System;

namespace Stashbox.Registration
{
    internal static class RegistrationBuilder
    {
        public static ServiceRegistration BuildServiceRegistration(IContainerContext containerContext, RegistrationConfiguration registrationConfiguration, bool isDecorator)
        {
            PreProcessExistingInstanceIfNeeded(containerContext, registrationConfiguration.Context, registrationConfiguration.ImplementationType);
            if (!isDecorator)
                registrationConfiguration.Context.Lifetime = ChooseLifeTime(containerContext, registrationConfiguration.Context);

            return registrationConfiguration.ImplementationType.IsOpenGenericType()
                ? new OpenGenericRegistration(registrationConfiguration.ImplementationType,
                    containerContext.ContainerConfiguration.RegistrationBehavior,
                    registrationConfiguration.Context, isDecorator)
                : new ServiceRegistration(registrationConfiguration.ImplementationType, DetermineRegistrationType(registrationConfiguration),
                    containerContext.ContainerConfiguration.RegistrationBehavior,
                    registrationConfiguration.Context, isDecorator);
        }

        private static void PreProcessExistingInstanceIfNeeded(IContainerContext containerContext, RegistrationContext registrationContext, Type implementationType)
        {
            if (registrationContext.ExistingInstance == null) return;

            if (!registrationContext.IsLifetimeExternallyOwned && implementationType.IsDisposable())
                containerContext.RootScope.AddDisposableTracking(registrationContext.ExistingInstance);

            if (registrationContext.Finalizer == null) return;
            containerContext.RootScope.AddWithFinalizer(registrationContext.ExistingInstance, registrationContext.Finalizer);
        }

        private static LifetimeDescriptor ChooseLifeTime(IContainerContext containerContext, RegistrationContext registrationContext) => registrationContext.IsWireUp
                ? Lifetimes.Singleton
                : registrationContext.Lifetime ?? containerContext.ContainerConfiguration.DefaultLifetime;

        private static RegistrationType DetermineRegistrationType(RegistrationConfiguration registrationConfiguration)
        {
            if (registrationConfiguration.Context.ExistingInstance != null)
                return registrationConfiguration.Context.IsWireUp
                    ? RegistrationType.WireUp
                    : RegistrationType.Instance;

            return registrationConfiguration.Context.Factory != null
                ? RegistrationType.Factory
                : RegistrationType.Default;
        }
    }
}
