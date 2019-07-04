using System;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// 
    /// </summary>
    public class DecoratorConfigurator : BaseFluentConfigurator<DecoratorConfigurator>
    {
        internal DecoratorConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        {
        }
    }
}
