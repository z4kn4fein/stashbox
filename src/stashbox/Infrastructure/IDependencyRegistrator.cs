using System;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a dependency registrator.
    /// </summary>
    public interface IDependencyRegistrator
    {
        /// <summary>
        /// Prepares a type for registration.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <returns>The created <see cref="IRegistrationContext"/> which allows further configurations.</returns>
        IRegistrationContext PrepareType<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Prepares a type for registration.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <returns>The created <see cref="IRegistrationContext"/> which allows further configurations.</returns>
        IRegistrationContext PrepareType<TFrom>(Type typeTo)
            where TFrom : class;

        /// <summary>
        /// Prepares a type for registration.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <returns>The created <see cref="IRegistrationContext"/> which allows further configurations.</returns>
        IRegistrationContext PrepareType(Type typeFrom, Type typeTo);

        /// <summary>
        /// Prepares a type for registration.
        /// </summary>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <returns>The created <see cref="IRegistrationContext"/> which allows further configurations.</returns>
        IRegistrationContext PrepareType<TTo>()
             where TTo : class;

        /// <summary>
        /// Registers a type into the container.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterType<TFrom, TTo>(string name = null)
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Registers a type into the container.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterType<TFrom>(Type typeTo, string name = null)
            where TFrom : class;

        /// <summary>
        /// Registers a type into the container.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterType(Type typeFrom, Type typeTo, string name = null);

        /// <summary>
        /// Registers a type into the container.
        /// </summary>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterType<TTo>(string name = null)
             where TTo : class;

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
        /// Registers an already constructed instance into the container.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="instance">The constructed object.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterInstance<TFrom>(object instance, string name = null)
            where TFrom : class;

        /// <summary>
        /// Registers an already constructed instance into the container.
        /// </summary>
        /// <param name="instance">The constructed object.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterInstance(object instance, string name = null);

        /// <summary>
        /// Registers an already constructed instance, but the container will perform injections and extensions on it.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="instance">The constructed object.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator BuildUp<TFrom>(object instance, string name = null)
            where TFrom : class;

        /// <summary>
        /// Registers an already constructed instance, but the container will perform injections and extensions on it.
        /// </summary>
        /// <param name="instance">The constructed object.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator BuildUp(object instance, string name = null);
    }
}
