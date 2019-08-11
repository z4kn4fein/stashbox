using System;

namespace Stashbox.Registration
{
    internal interface IServiceRegistrator
    {
        void Register(IServiceRegistration serviceRegistration, Type serviceType, RegistrationContext registrationContext);

        void Register(IServiceRegistration serviceRegistration, Type serviceType, bool replace);

        void ReMap(IServiceRegistration serviceRegistration, Type serviceType, RegistrationContext registrationContext);
    }
}
