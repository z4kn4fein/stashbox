using System;

namespace Stashbox
{
    /// <summary>
    /// Represents the extension methods of <see cref="IDependencyRegistrator"/>.
    /// </summary>
    public static class DependencyRegistratorExtensions
    {
        /// <summary>
        /// Registers an already constructed instance into the container.
        /// </summary>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="instance">The constructed object.</param>
        /// <param name="name">The name of the registration.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from the disposal tracking.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer RegisterInstance(this IDependencyRegistrator registrator, object instance, object name = null, bool withoutDisposalTracking = false) =>
            registrator.RegisterInstance(instance.GetType(), instance, name, withoutDisposalTracking);

        /// <summary>
        /// Registers an already constructed instance, but the container will perform injections and extensions on it.
        /// </summary>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="instance">The constructed object.</param>
        /// <param name="name">The name of the registration.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from the disposal tracking.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer WireUp(this IDependencyRegistrator registrator, object instance, object name = null, bool withoutDisposalTracking = false) =>
            registrator.WireUp(instance.GetType(), instance, name, withoutDisposalTracking);

        /// <summary>
        /// Registers a type with singleton lifetime.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer RegisterSingleton<TFrom, TTo>(this IDependencyRegistrator registrator, object name = null)
            where TFrom : class
            where TTo : class, TFrom =>
            registrator.RegisterType<TFrom, TTo>(context => context.WithName(name).WithSingletonLifetime());

        /// <summary>
        /// Registers a type with singleton lifetime.
        /// </summary>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer RegisterSingleton<TTo>(this IDependencyRegistrator registrator, object name = null)
            where TTo : class =>
            registrator.RegisterType<TTo>(context => context.WithName(name).WithSingletonLifetime());

        /// <summary>
        /// Registers a type with singleton lifetime.
        /// </summary>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer RegisterSingleton(this IDependencyRegistrator registrator, Type typeFrom, Type typeTo, object name = null) =>
            registrator.RegisterType(typeFrom, typeTo, context => context.WithName(name).WithSingletonLifetime());

        /// <summary>
        /// Registers a type with scoped lifetime.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer RegisterScoped<TFrom, TTo>(this IDependencyRegistrator registrator, object name = null)
            where TFrom : class
            where TTo : class, TFrom =>
            registrator.RegisterType<TFrom, TTo>(context => context.WithName(name).WithScopedLifetime());

        /// <summary>
        /// Registers a type with scoped lifetime.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer RegisterScoped(this IDependencyRegistrator registrator, Type typeFrom, Type typeTo, object name = null) =>
            registrator.RegisterType(typeFrom, typeTo, context => context.WithName(name).WithScopedLifetime());

        /// <summary>
        /// Registers a type with scoped lifetime.
        /// </summary>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer RegisterScoped<TTo>(this IDependencyRegistrator registrator, object name = null)
            where TTo : class =>
            registrator.RegisterType<TTo>(context => context.WithName(name).WithScopedLifetime());
    }
}
