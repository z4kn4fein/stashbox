using System;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// 
    /// </summary>
    public class DecoratorRegistrationContext : BaseFluentConfigurator<DecoratorRegistrationContext>
    {
        internal DecoratorRegistrationContext(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        {
        }
    }
}
