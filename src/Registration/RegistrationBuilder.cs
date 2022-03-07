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

            return DetermineRegistrationType(registrationConfiguration, containerContext, isDecorator);
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

        private static ServiceRegistration DetermineRegistrationType(RegistrationConfiguration registrationConfiguration,
            IContainerContext containerContext, bool isDecorator)
        {
            if (registrationConfiguration.ImplementationType.IsOpenGenericType())
                return new OpenGenericRegistration(registrationConfiguration.ImplementationType,
                    registrationConfiguration.Context, containerContext.ContainerConfiguration, isDecorator);
            
            if (registrationConfiguration.Context.ExistingInstance != null)
                return new InstanceRegistration(registrationConfiguration.ImplementationType,
                    registrationConfiguration.Context, containerContext.ContainerConfiguration, isDecorator,
                    registrationConfiguration.Context.ExistingInstance);

            if (registrationConfiguration.Context.Factory != null && registrationConfiguration.Context.FactoryParameters != null)
                return new FactoryRegistration(registrationConfiguration.ImplementationType,
                    registrationConfiguration.Context, containerContext.ContainerConfiguration, isDecorator,
                    registrationConfiguration.Context.Factory, registrationConfiguration.Context.FactoryParameters);

            return new ServiceRegistration(registrationConfiguration.ImplementationType,
                    registrationConfiguration.Context, containerContext.ContainerConfiguration, isDecorator);
        }
    }
}
