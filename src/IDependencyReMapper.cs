using Stashbox.Registration.Fluent;
using System;

namespace Stashbox
{
    /// <summary>
    /// Represents a dependency remapper.
    /// </summary>
    public interface IDependencyReMapper
    {
        /// <summary>
        /// Replaces an existing registration mapping.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer ReMap<TFrom, TTo>(Action<IFluentServiceConfigurator<TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Replaces an existing registration mapping.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer ReMap<TFrom>(Type typeTo, Action<IFluentServiceConfigurator<TFrom>> configurator = null)
            where TFrom : class;

        /// <summary>
        /// Replaces an existing registration mapping.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer ReMap(Type typeFrom, Type typeTo, Action<IFluentServiceConfigurator> configurator = null);

        /// <summary>
        /// Replaces an existing registration mapping.
        /// </summary>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer ReMap<TTo>(Action<IFluentServiceConfigurator<TTo>> configurator = null)
             where TTo : class;

        /// <summary>
        /// Replaces an existing decorator mapping.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer ReMapDecorator(Type typeFrom, Type typeTo, Action<IFluentDecoratorConfigurator> configurator = null);
    }
}
