using System;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// 
    /// </summary>
    public class RegistrationConfigurator<TService> : FluentServiceConfigurator<TService, RegistrationConfigurator<TService>>
    {
        internal RegistrationConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RegistrationConfigurator : FluentServiceConfigurator<RegistrationConfigurator>
    {
        internal RegistrationConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        {
        }
    }
}
