using System;

namespace Stashbox.Registration
{
    internal interface IServiceRegistrator
    {
        IStashboxContainer Register(IServiceRegistration serviceRegistration, IRegistrationContext registrationContext);

        IStashboxContainer Register(IServiceRegistration serviceRegistration, Type serviceType, bool replace);

        IStashboxContainer ReMap(IServiceRegistration serviceRegistration, IRegistrationContext registrationContext);
    }
}
