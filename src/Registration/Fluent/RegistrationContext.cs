using System;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// 
    /// </summary>
    public class RegistrationContext<TService> : FluentServiceConfigurator<TService, RegistrationContext<TService>>
    {
        internal RegistrationContext(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RegistrationContext : FluentServiceConfigurator<RegistrationContext>
    {
        internal RegistrationContext(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        {
        }
    }
}
