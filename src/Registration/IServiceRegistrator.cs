using System;

namespace Stashbox.Registration
{
    internal interface IServiceRegistrator
    {
        void Register(IContainerContext containerContext, IServiceRegistration serviceRegistration, Type serviceType, RegistrationContext registrationContext);

        void Register(IContainerContext containerContext, IServiceRegistration serviceRegistration, Type serviceType, bool replace);

        void ReMap(IContainerContext containerContext, IServiceRegistration serviceRegistration, Type serviceType, RegistrationContext registrationContext);
    }
}
