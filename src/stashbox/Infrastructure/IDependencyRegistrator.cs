using System;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a dependency registrator.
    /// </summary>
    public interface IDependencyRegistrator : IDependencyReMapper, IDependencyCollectionRegistrator
    {
        /// <summary>
        /// Registers a type into the container with custom configuration.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterType<TFrom, TTo>(Action<IFluentServiceRegistrator> configurator = null)
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Registers a type into the container with custom configuration.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterType<TFrom>(Type typeTo, Action<IFluentServiceRegistrator> configurator = null)
            where TFrom : class;

        /// <summary>
        /// Registers a type into the container with custom configuration.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterType(Type typeFrom, Type typeTo, Action<IFluentServiceRegistrator> configurator = null);

        /// <summary>
        /// Registers a type into the container with custom configuration.
        /// </summary>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterType<TTo>(Action<IFluentServiceRegistrator> configurator = null)
             where TTo : class;

        /// <summary>
        /// Registers a type into the container with custom configuration.
        /// </summary>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterType(Type typeTo, Action<IFluentServiceRegistrator> configurator = null);

        /// <summary>
        /// Registers an already constructed instance into the container.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="instance">The constructed object.</param>
        /// <param name="name">The name of the registration.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from the disposal tracking.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterInstanceAs<TFrom>(TFrom instance, object name = null, bool withoutDisposalTracking = false);

        /// <summary>
        /// Registers an already constructed instance into the container.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="instance">The constructed object.</param>
        /// <param name="name">The name of the registration.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from the disposal tracking.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterInstance(Type serviceType, object instance, object name = null, bool withoutDisposalTracking = false);

        /// <summary>
        /// Registers an already constructed instance into the container.
        /// </summary>
        /// <param name="instance">The constructed object.</param>
        /// <param name="name">The name of the registration.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from the disposal tracking.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterInstance(object instance, object name = null, bool withoutDisposalTracking = false);

        /// <summary>
        /// Registers an already constructed instance, but the container will perform injections and extensions on it.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="instance">The constructed object.</param>
        /// <param name="name">The name of the registration.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from the disposal tracking.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator WireUpAs<TFrom>(TFrom instance, object name = null, bool withoutDisposalTracking = false);

        /// <summary>
        /// Registers an already constructed instance, but the container will perform injections and extensions on it.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="instance">The constructed object.</param>
        /// <param name="name">The name of the registration.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from the disposal tracking.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator WireUp(Type serviceType, object instance, object name = null, bool withoutDisposalTracking = false);

        /// <summary>
        /// Registers an already constructed instance, but the container will perform injections and extensions on it.
        /// </summary>
        /// <param name="instance">The constructed object.</param>
        /// <param name="name">The name of the registration.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from the disposal tracking.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator WireUp(object instance, object name = null, bool withoutDisposalTracking = false);

        /// <summary>
        /// Registers a type with singleton lifetime.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterSingleton<TFrom, TTo>(object name = null)
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Registers a type with singleton lifetime.
        /// </summary>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterSingleton<TTo>(object name = null)
            where TTo : class;

        /// <summary>
        /// Registers a type with singleton lifetime.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterSingleton(Type typeFrom, Type typeTo, object name = null);

        /// <summary>
        /// Registers a type with scoped lifetime.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterScoped<TFrom, TTo>(object name = null)
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Registers a type with scoped lifetime.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterScoped(Type typeFrom, Type typeTo, object name = null);

        /// <summary>
        /// Registers a type with scoped lifetime.
        /// </summary>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterScoped<TTo>(object name = null)
            where TTo : class;
    }
}
