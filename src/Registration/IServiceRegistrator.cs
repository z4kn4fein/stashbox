using System;

namespace Stashbox.Registration
{
    internal interface IServiceRegistrator
    {
        IStashboxContainer Register(IServiceRegistration serviceRegistration, Type serviceType, RegistrationContext registrationContext);

        IStashboxContainer Register(IServiceRegistration serviceRegistration, Type serviceType, bool replace);

        IStashboxContainer ReMap(IServiceRegistration serviceRegistration, Type serviceType, RegistrationContext registrationContext);
    }
}
