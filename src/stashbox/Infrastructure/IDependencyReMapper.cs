using System;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.Infrastructure
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
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator ReMap<TFrom, TTo>(Action<IFluentServiceRegistrator<TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Replaces an existing registration mapping.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator ReMap<TFrom>(Type typeTo, Action<IFluentServiceRegistrator<TFrom>> configurator = null)
            where TFrom : class;

        /// <summary>
        /// Replaces an existing registration mapping.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator ReMap(Type typeFrom, Type typeTo, Action<IFluentServiceRegistrator> configurator = null);

        /// <summary>
        /// Replaces an existing registration mapping.
        /// </summary>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator ReMap<TTo>(Action<IFluentServiceRegistrator<TTo>> configurator = null)
             where TTo : class;

        /// <summary>
        /// Replaces an existing registration mapping.
        /// </summary>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator ReMap(Type typeTo, Action<IFluentServiceRegistrator> configurator = null);

        /// <summary>
        /// Replaces an existing decorator mapping.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator ReMapDecorator<TFrom, TTo>(Action<IFluentDecoratorRegistrator> configurator = null)
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Replaces an existing decorator mapping.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator ReMapDecorator<TFrom>(Type typeTo, Action<IFluentDecoratorRegistrator> configurator = null)
            where TFrom : class;

        /// <summary>
        /// Replaces an existing decorator mapping.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator ReMapDecorator(Type typeFrom, Type typeTo, Action<IFluentDecoratorRegistrator> configurator = null);
    }
}
