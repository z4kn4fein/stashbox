using System;
using System.Linq;

namespace Stashbox.Registration
{
    internal static class ServiceRegistrator
    {
        public static void Register(IContainerContext containerContext, ServiceRegistration serviceRegistration, Type serviceType)
        {
            if (serviceRegistration.AdditionalServiceTypes != null)
                foreach (var additionalServiceType in serviceRegistration.AdditionalServiceTypes.Distinct())
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

        public static void ReMap(IContainerContext containerContext, ServiceRegistration serviceRegistration, Type serviceType)
        {
            if (serviceRegistration.AdditionalServiceTypes != null)
                foreach (var additionalServiceType in serviceRegistration.AdditionalServiceTypes.Distinct())
                    ReMapInternal(containerContext, serviceRegistration, additionalServiceType);

            ReMapInternal(containerContext, serviceRegistration, serviceType);
        }

        private static void ReMapInternal(IContainerContext containerContext, ServiceRegistration serviceRegistration, Type serviceType)
        {
            if (serviceRegistration.IsDecorator)
                containerContext.DecoratorRepository.AddDecorator(serviceType, serviceRegistration, true);
            else
                containerContext.RegistrationRepository.AddOrReMapRegistration(serviceRegistration, serviceType);

            containerContext.RootScope.InvalidateDelegateCache();
        }
    }
}
