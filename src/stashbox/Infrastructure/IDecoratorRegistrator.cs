using System;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a decorator registrator.
    /// </summary>
    public interface IDecoratorRegistrator
    {
        /// <summary>
        /// Registers a decorator type into the container with custom configuration.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDecoratorRegistrator"/> which on this method was called.</returns>
        IDecoratorRegistrator RegisterDecorator<TFrom, TTo>(Action<IFluentDecoratorRegistrator> configurator = null)
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Registers a decorator type into the container with custom configuration.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDecoratorRegistrator"/> which on this method was called.</returns>
        IDecoratorRegistrator RegisterDecorator<TFrom>(Type typeTo, Action<IFluentDecoratorRegistrator> configurator = null)
            where TFrom : class;

        /// <summary>
        /// Registers a decorator type into the container with custom configuration.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDecoratorRegistrator"/> which on this method was called.</returns>
        IDecoratorRegistrator RegisterDecorator(Type typeFrom, Type typeTo, Action<IFluentDecoratorRegistrator> configurator = null);
    }
}
