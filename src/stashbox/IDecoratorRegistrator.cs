using Stashbox.Registration;
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
        IStashboxContainer RegisterDecorator(Type typeFrom, Type typeTo, Action<IFluentDecoratorRegistrator> configurator = null);
    }
}
