using Stashbox.Lifetime;
using Stashbox.Registration.ServiceRegistrations;
using System;
using System.Linq;

namespace Stashbox.Registration
{
    internal static class ServiceRegistrator
    {
        public static void Register(IContainerContext containerContext, ServiceRegistration serviceRegistration, Type serviceType)
        {
            if (serviceRegistration.ImplementationType.IsOpenGenericType())
                serviceRegistration = RegistrationFactory.EnsureOpenGeneric(serviceRegistration);

            PreProcessRegistration(containerContext, serviceRegistration);

            if (serviceRegistration is ComplexRegistration complexRegistration && complexRegistration.AdditionalServiceTypes != null)
                foreach (var additionalServiceType in complexRegistration.AdditionalServiceTypes.Distinct())
                {
                    if (additionalServiceType.IsOpenGenericType())
                    {
                        RegisterInternal(containerContext, serviceRegistration, additionalServiceType.GetGenericTypeDefinition());
                        continue;
                    }

                    RegisterInternal(containerContext, serviceRegistration, additionalServiceType);
                }

            RegisterInternal(containerContext, serviceRegistration, serviceType);
        }

        public static void ReMap(IContainerContext containerContext, ServiceRegistration serviceRegistration, Type serviceType)
        {
            if (serviceRegistration.ImplementationType.IsOpenGenericType())
                serviceRegistration = RegistrationFactory.EnsureOpenGeneric(serviceRegistration);

            PreProcessRegistration(containerContext, serviceRegistration);

            if (serviceRegistration is ComplexRegistration complexRegistration && complexRegistration.AdditionalServiceTypes != null)
                foreach (var additionalServiceType in complexRegistration.AdditionalServiceTypes.Distinct())
                    ReMapInternal(containerContext, serviceRegistration, additionalServiceType);

            ReMapInternal(containerContext, serviceRegistration, serviceType);
        }

        private static void RegisterInternal(IContainerContext containerContext, ServiceRegistration serviceRegistration, Type serviceType)
        {
            if (serviceRegistration.IsDecorator)
            {
                containerContext.DecoratorRepository.AddDecorator(serviceType, serviceRegistration, false);
                containerContext.RootScope.InvalidateDelegateCache();
            }
            else if (containerContext.RegistrationRepository.AddOrUpdateRegistration(serviceRegistration, serviceType))
                containerContext.RootScope.InvalidateDelegateCache();
        }

        private static void ReMapInternal(IContainerContext containerContext, ServiceRegistration serviceRegistration, Type serviceType)
        {
            if (serviceRegistration.IsDecorator)
                containerContext.DecoratorRepository.AddDecorator(serviceType, serviceRegistration, true);
            else
                containerContext.RegistrationRepository.AddOrReMapRegistration(serviceRegistration, serviceType);

            containerContext.RootScope.InvalidateDelegateCache();
        }

        private static void PreProcessRegistration(IContainerContext containerContext, ServiceRegistration serviceRegistration)
        {
            if (serviceRegistration is InstanceRegistration instanceRegistration)
            {
                PreProcessExistingInstanceIfNeeded(containerContext, instanceRegistration.ExistingInstance, instanceRegistration.IsLifetimeExternallyOwned, 
                    instanceRegistration.Finalizer, instanceRegistration.ImplementationType);

                if (instanceRegistration.IsWireUp)
                    instanceRegistration.Lifetime = Lifetimes.Singleton;
            }
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
    }
}
