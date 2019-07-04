using Stashbox.Registration.Fluent;
using System;

namespace Stashbox.Registration
{
    internal interface IServiceRegistrator
    {
        IStashboxContainer Register(IServiceRegistration serviceRegistration, RegistrationConfigurator registrationContext);

        IStashboxContainer Register(IServiceRegistration serviceRegistration, Type serviceType, bool replace);

        IStashboxContainer ReMap(IServiceRegistration serviceRegistration, RegistrationConfigurator registrationContext);
    }
}
