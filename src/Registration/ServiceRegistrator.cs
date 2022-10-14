using Stashbox.Lifetime;
using Stashbox.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.Registration
{
    internal static class ServiceRegistrator
    {
        public static void Register(IContainerContext containerContext, ServiceRegistration serviceRegistration, Type serviceType)
        {
            if (serviceRegistration.ImplementationType.IsOpenGenericType())
                serviceRegistration = new OpenGenericRegistration(serviceRegistration);

            PreProcessRegistration(containerContext, serviceRegistration);

            if (serviceRegistration.Options.TryGet(RegistrationOption.AdditionalServiceTypes, out var types) && types is ExpandableArray<Type> additionalTypes)
                foreach (var additionalServiceType in additionalTypes.Distinct())
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
                serviceRegistration = new OpenGenericRegistration(serviceRegistration);

            PreProcessRegistration(containerContext, serviceRegistration);

            if (serviceRegistration.Options.TryGet(RegistrationOption.AdditionalServiceTypes, out var types) && types is ExpandableArray<Type> additionalTypes)
                foreach (var additionalServiceType in additionalTypes.Distinct())
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
            if (serviceRegistration.Options.TryGet(RegistrationOption.RegistrationTypeOptions, out var opts) && opts is InstanceOptions instanceOptions)
            {
                PreProcessExistingInstanceIfNeeded(containerContext, instanceOptions.ExistingInstance, serviceRegistration.Options.IsOn(RegistrationOption.IsLifetimeExternallyOwned), 
                    serviceRegistration.Options.GetOrDefault<Action<object>>(RegistrationOption.Finalizer), serviceRegistration.ImplementationType);

                if (instanceOptions.IsWireUp)
                    serviceRegistration.Lifetime = Lifetimes.Singleton;
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
