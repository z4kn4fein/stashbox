using Stashbox.Utils;
using System;
using System.Collections.Generic;

namespace Stashbox
{
    /// <summary>
    /// Represents the extension methods of <see cref="IDependencyRegistrator"/>.
    /// </summary>
    public static class DependencyRegistratorExtensions
    {
        /// <summary>
        /// Registers a type with a name.
        /// </summary>
        /// <typeparam name="TFrom">The service type.</typeparam>
        /// <typeparam name="TTo">The implementation type.</typeparam>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        public static IStashboxContainer Register<TFrom, TTo>(this IDependencyRegistrator registrator, object name = null)
            where TFrom : class
            where TTo : class, TFrom =>
            registrator.Register<TFrom, TTo>(context => context.WithName(name));

        /// <summary>
        /// Registers instances that are already constructed.
        /// </summary>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="instances">The collection of the constructed instances.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from the disposal tracking.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        public static IStashboxContainer RegisterInstances<TFrom>(this IDependencyRegistrator registrator,
            IEnumerable<TFrom> instances, bool withoutDisposalTracking = false)
            where TFrom : class
        {
            Shield.EnsureNotNull(instances, nameof(instances));

            foreach (var instance in instances)
                registrator.RegisterInstance(instance, withoutDisposalTracking: withoutDisposalTracking);

            return (IStashboxContainer)registrator;
        }

        /// <summary>
        /// Registers instances that are already constructed.
        /// </summary>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="instances">The collection of the constructed instances.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        public static IStashboxContainer RegisterInstances<TFrom>(this IDependencyRegistrator registrator,
            params TFrom[] instances)
            where TFrom : class => registrator.RegisterInstances(instances, false);

        /// <summary>
        /// Registers a type with singleton lifetime.
        /// </summary>
        /// <typeparam name="TFrom">The service type.</typeparam>
        /// <typeparam name="TTo">The implementation type.</typeparam>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        public static IStashboxContainer RegisterSingleton<TFrom, TTo>(this IDependencyRegistrator registrator, object name = null)
            where TFrom : class
            where TTo : class, TFrom =>
            registrator.Register<TFrom, TTo>(context => context.WithName(name).WithSingletonLifetime());

        /// <summary>
        /// Registers a type with singleton lifetime.
        /// </summary>
        /// <typeparam name="TTo">The service/implementation type.</typeparam>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        public static IStashboxContainer RegisterSingleton<TTo>(this IDependencyRegistrator registrator, object name = null)
            where TTo : class =>
            registrator.Register<TTo>(context => context.WithName(name).WithSingletonLifetime());

        /// <summary>
        /// Registers a type with singleton lifetime.
        /// </summary>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="typeFrom">The service type.</param>
        /// <param name="typeTo">The implementation type.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        public static IStashboxContainer RegisterSingleton(this IDependencyRegistrator registrator,
            Type typeFrom, Type typeTo, object name = null) =>
            registrator.Register(typeFrom, typeTo, context => context.WithName(name).WithSingletonLifetime());

        /// <summary>
        /// Registers a type with scoped lifetime.
        /// </summary>
        /// <typeparam name="TFrom">The service type.</typeparam>
        /// <typeparam name="TTo">The implementation type.</typeparam>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        public static IStashboxContainer RegisterScoped<TFrom, TTo>(this IDependencyRegistrator registrator, object name = null)
            where TFrom : class
            where TTo : class, TFrom =>
            registrator.Register<TFrom, TTo>(context => context.WithName(name).WithScopedLifetime());

        /// <summary>
        /// Registers a type with scoped lifetime.
        /// </summary>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="typeFrom">The service type.</param>
        /// <param name="typeTo">The implementation type.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        public static IStashboxContainer RegisterScoped(this IDependencyRegistrator registrator,
            Type typeFrom, Type typeTo, object name = null) =>
            registrator.Register(typeFrom, typeTo, context => context.WithName(name).WithScopedLifetime());

        /// <summary>
        /// Registers a type with scoped lifetime.
        /// </summary>
        /// <typeparam name="TTo">The implementation type.</typeparam>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        public static IStashboxContainer RegisterScoped<TTo>(this IDependencyRegistrator registrator, object name = null)
            where TTo : class =>
            registrator.Register<TTo>(context => context.WithName(name).WithScopedLifetime());
    }
}
