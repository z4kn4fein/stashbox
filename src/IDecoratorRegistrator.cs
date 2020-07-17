using Stashbox.Registration.Fluent;
using System;

namespace Stashbox
{
    /// <summary>
    /// Represents a decorator registrator.
    /// </summary>
    public interface IDecoratorRegistrator
    {
        /// <summary>
        /// Registers a decorator type into the container with custom configuration.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer RegisterDecorator(Type typeFrom, Type typeTo, Action<DecoratorConfigurator> configurator = null);

        /// <summary>
        /// Registers a decorator type into the container with custom configuration.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer RegisterDecorator<TFrom, TTo>(Action<DecoratorConfigurator<TFrom, TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Registers a decorator type into the container with custom configuration.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer RegisterDecorator<TFrom>(Type typeTo,
            Action<DecoratorConfigurator<TFrom, TFrom>> configurator = null)
            where TFrom : class;
    }
}
