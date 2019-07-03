using Stashbox.Registration.Fluent;
using System;

namespace Stashbox.Registration
{
    internal interface IServiceRegistrator
    {
        IStashboxContainer Register(IServiceRegistration serviceRegistration, RegistrationContext registrationContext);

        IStashboxContainer Register(IServiceRegistration serviceRegistration, Type serviceType, bool replace);

        IStashboxContainer ReMap(IServiceRegistration serviceRegistration, RegistrationContext registrationContext);
    }
}
