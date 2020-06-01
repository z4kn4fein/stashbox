using System;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// Represents the fluent service decorator registration api.
    /// </summary>
    public class DecoratorConfigurator : BaseFluentConfigurator<DecoratorConfigurator>
    {
        internal DecoratorConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        {
        }
    }
}
