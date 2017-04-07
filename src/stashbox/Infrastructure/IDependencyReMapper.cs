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
        /// Replaces an existing registration with a new one.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator ReMap<TFrom, TTo>(string name = null)
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Replaces an existing registration with a new one.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator ReMap<TFrom>(Type typeTo, string name = null)
            where TFrom : class;

        /// <summary>
        /// Replaces an existing registration with a new one.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator ReMap(Type typeFrom, Type typeTo, string name = null);

        /// <summary>
        /// Replaces an existing registration with a new one.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator ReMap<TFrom, TTo>(Action<IFluentServiceRegistrator> configurator)
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Replaces an existing registration with a new one.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator ReMap<TFrom>(Type typeTo, Action<IFluentServiceRegistrator> configurator)
            where TFrom : class;

        /// <summary>
        /// Replaces an existing registration with a new one.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator ReMap(Type typeFrom, Type typeTo, Action<IFluentServiceRegistrator> configurator);

        /// <summary>
        /// Replaces an existing registration with a new one.
        /// </summary>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator ReMap<TTo>(Action<IFluentServiceRegistrator> configurator)
             where TTo : class;

        /// <summary>
        /// Replaces an existing registration with a new one.
        /// </summary>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator ReMap(Type typeTo, Action<IFluentServiceRegistrator> configurator);
    }
}
