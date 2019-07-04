using System;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// Represents the generic fluent service registration api.
    /// </summary>
    public class RegistrationConfigurator<TService> : FluentServiceConfigurator<TService, RegistrationConfigurator<TService>>
    {
        internal RegistrationConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        {
        }
    }

    /// <summary>
    /// Represents the fluent service registraton api.
    /// </summary>
    public class RegistrationConfigurator : FluentServiceConfigurator<RegistrationConfigurator>
    {
        internal RegistrationConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        {
        }
    }
}
