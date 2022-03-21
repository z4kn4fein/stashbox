using Stashbox.Lifetime;
using Stashbox.Registration.Fluent;
using System;

namespace Stashbox.Registration
{
    internal static class RegistrationBuilder
    {
        public static ServiceRegistration BuildServiceRegistration(IContainerContext containerContext, RegistrationConfiguration registrationConfiguration, bool isDecorator)
        {
            PreProcessExistingInstanceIfNeeded(containerContext, registrationConfiguration.Context.ExistingInstance,
                registrationConfiguration.Context.IsLifetimeExternallyOwned, registrationConfiguration.Context.Finalizer,
                registrationConfiguration.ImplementationType);

            if (!isDecorator)
                registrationConfiguration.Context.Lifetime = ChooseLifeTime(containerContext, registrationConfiguration.Context);

            return DetermineRegistrationType(registrationConfiguration, containerContext, isDecorator);
        }

        public static ServiceRegistration BuildServiceRegistration(IContainerContext containerContext, Type implementationType,
            object? name, LifetimeDescriptor? lifetime, bool isDecorator) =>
            implementationType.IsOpenGenericType()
                ? new OpenGenericRegistration(implementationType, containerContext.ContainerConfiguration, name, lifetime, isDecorator)
                : new ServiceRegistration(implementationType, containerContext.ContainerConfiguration, isDecorator, name, lifetime);

        public static ServiceRegistration BuildInstanceRegistration(IContainerContext containerContext, Type implementationType, object? name,
            object instance, bool isWireUp, bool isLifetimeExternallyOwned, Action<object>? finalizerDelegate)
        {
            PreProcessExistingInstanceIfNeeded(containerContext, instance,
                isLifetimeExternallyOwned, finalizerDelegate, implementationType);

            return new InstanceRegistration(implementationType, name, instance, isWireUp,
                isLifetimeExternallyOwned, finalizerDelegate, containerContext.ContainerConfiguration);
        }

        private static void PreProcessExistingInstanceIfNeeded(IContainerContext containerContext, object? instance,
            bool isLifetimeExternallyOwned, Action<object>? finalizer, Type implementationType)
        {
            if (instance == null) return;

            if (!isLifetimeExternallyOwned && implementationType.IsDisposable())
                containerContext.RootScope.AddDisposableTracking(instance);

            if (finalizer == null) return;
            containerContext.RootScope.AddWithFinalizer(instance, finalizer);
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
