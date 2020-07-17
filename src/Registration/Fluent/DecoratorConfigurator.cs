using System;
using System.Linq.Expressions;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// Represents the fluent service decorator registration api.
    /// </summary>
    public class DecoratorConfigurator<TService, TImplementation> : BaseFluentServiceConfigurator<TService, TImplementation, DecoratorConfigurator<TService, TImplementation>>
    {
        internal DecoratorConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        { }
    }

    /// <summary>
    /// Represents the fluent service decorator registration api.
    /// </summary>
    public class DecoratorConfigurator : BaseFluentConfigurator<DecoratorConfigurator>
    {
        internal DecoratorConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        { }

        /// <summary>
        /// Sets a container factory delegate for the registration.
        /// </summary>
        /// <param name="containerFactory">The container factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public DecoratorConfigurator WithFactory(Func<IDependencyResolver, object> containerFactory, bool isCompiledLambda = false)
        {
            this.Context.ContainerFactory = containerFactory;
            this.Context.IsFactoryDelegateACompiledLambda = isCompiledLambda;
            return this;
        }

        /// <summary>
        /// Sets a parameter-less factory delegate for the registration.
        /// </summary>
        /// <param name="singleFactory">The factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public DecoratorConfigurator WithFactory(Func<object> singleFactory, bool isCompiledLambda = false)
        {
            this.Context.SingleFactory = singleFactory;
            this.Context.IsFactoryDelegateACompiledLambda = isCompiledLambda;
            return this;
        }
    }
}
