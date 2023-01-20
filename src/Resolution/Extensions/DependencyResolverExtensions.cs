using System;
using Stashbox.Utils;

namespace Stashbox;

/// <summary>
/// Represents the extensions of the <see cref="IDependencyResolver"/>.
/// </summary>
public static class DependencyResolverExtensions
{
    /// <summary>
    /// Resolves an instance from the container.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <returns>The resolved object.</returns>
    public static TKey Resolve<TKey>(this IDependencyResolver resolver) =>
        (TKey)resolver.Resolve(TypeCache<TKey>.Type);

    /// <summary>
    /// Resolves an instance from the container with dependency overrides.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
    /// <returns>The resolved object.</returns>
    public static TKey Resolve<TKey>(this IDependencyResolver resolver, object[] dependencyOverrides) =>
        (TKey)resolver.Resolve(TypeCache<TKey>.Type, dependencyOverrides);

    /// <summary>
    /// Resolves a named instance from the container.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="name">The name of the requested registration.</param>
    /// <returns>The resolved object.</returns>
    public static TKey Resolve<TKey>(this IDependencyResolver resolver, object? name) =>
        (TKey)resolver.Resolve(TypeCache<TKey>.Type, name);

    /// <summary>
    /// Resolves a named instance from the container with dependency overrides.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="name">The name of the requested registration.</param>
    /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
    /// <returns>The resolved object.</returns>
    public static TKey Resolve<TKey>(this IDependencyResolver resolver, object? name, object[] dependencyOverrides) =>
        (TKey)resolver.Resolve(TypeCache<TKey>.Type, name, dependencyOverrides);

    /// <summary>
    /// Resolves a named instance from the container or returns default if the type is not resolvable.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="name">The name of the requested registration.</param>
    /// <returns>The resolved object.</returns>
    public static TKey? ResolveOrDefault<TKey>(this IDependencyResolver resolver, object? name) =>
        (TKey?)(resolver.ResolveOrDefault(TypeCache<TKey>.Type, name) ?? default(TKey));

    /// <summary>
    /// Resolves a named instance from the container with dependency overrides or returns default if the type is not resolvable.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="name">The name of the requested registration.</param>
    /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
    /// <returns>The resolved object.</returns>
    public static TKey? ResolveOrDefault<TKey>(this IDependencyResolver resolver, object? name, object[] dependencyOverrides) =>
        (TKey?)(resolver.ResolveOrDefault(TypeCache<TKey>.Type, name, dependencyOverrides) ?? default(TKey));

    /// <summary>
    /// Resolves an instance from the container or returns default if the type is not resolvable.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <returns>The resolved object.</returns>
    public static TKey? ResolveOrDefault<TKey>(this IDependencyResolver resolver) =>
        (TKey?)(resolver.ResolveOrDefault(TypeCache<TKey>.Type) ?? default(TKey));

    /// <summary>
    /// Resolves an instance from the container with dependency overrides or returns default if the type is not resolvable.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
    /// <returns>The resolved object.</returns>
    public static TKey? ResolveOrDefault<TKey>(this IDependencyResolver resolver, object[] dependencyOverrides) =>
        (TKey?)(resolver.ResolveOrDefault(TypeCache<TKey>.Type, dependencyOverrides) ?? default(TKey));

    /// <summary>
    /// On the fly activates an object without registering it into the container. If you want to resolve a
    /// registered service use the <see cref="IDependencyResolver.Resolve(Type, object[])" /> instead.
    /// </summary>
    /// <typeparam name="TTo">The service type.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="arguments">Optional dependency overrides.</param>
    /// <returns>The built object.</returns>
    public static TTo Activate<TTo>(this IDependencyResolver resolver, params object[] arguments) =>
        (TTo)resolver.Activate(TypeCache<TTo>.Type, arguments);

    /// <summary>
    /// Puts an instance into the scope which will be dropped when the scope is being disposed.
    /// </summary>
    /// <typeparam name="TFrom">The service type.</typeparam>
    /// <param name="resolver">The resolver.</param>
    /// <param name="instance">The instance.</param>
    /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from the disposal tracking.</param>
    /// <param name="name">The identifier.</param>
    /// <returns>The scope.</returns>
    public static void PutInstanceInScope<TFrom>(this IDependencyResolver resolver, TFrom instance, bool withoutDisposalTracking = false, object? name = null)
        where TFrom : class =>
        resolver.PutInstanceInScope(TypeCache<TFrom>.Type, instance, withoutDisposalTracking, name);
}