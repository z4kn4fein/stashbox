using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Stashbox.Resolution;
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
    /// Resolves an instance from the container.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static TKey Resolve<TKey>(this IDependencyResolver resolver, ResolutionBehavior resolutionBehavior) =>
        (TKey)resolver.Resolve(TypeCache<TKey>.Type, null, null, resolutionBehavior);

    /// <summary>
    /// Resolves an instance from the container with dependency overrides.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static TKey Resolve<TKey>(this IDependencyResolver resolver, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        (TKey)resolver.Resolve(TypeCache<TKey>.Type, null, dependencyOverrides, resolutionBehavior);

    /// <summary>
    /// Resolves a named instance from the container.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="name">The name of the requested registration.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static TKey Resolve<TKey>(this IDependencyResolver resolver, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        (TKey)resolver.Resolve(TypeCache<TKey>.Type, name, null, resolutionBehavior);

    /// <summary>
    /// Resolves a named instance from the container with dependency overrides.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="name">The name of the requested registration.</param>
    /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static TKey Resolve<TKey>(this IDependencyResolver resolver, object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        (TKey)resolver.Resolve(TypeCache<TKey>.Type, name, dependencyOverrides, resolutionBehavior);

    /// <summary>
    /// Resolves an instance from the container.
    /// </summary>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="typeFrom">The type of the requested service.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static object Resolve(this IDependencyResolver resolver, Type typeFrom, ResolutionBehavior resolutionBehavior) =>
        resolver.Resolve(typeFrom, null, null, resolutionBehavior);
    
    /// <summary>
    /// Resolves an instance from the container with dependency overrides.
    /// </summary>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="typeFrom">The type of the requested service.</param>
    /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static object Resolve(this IDependencyResolver resolver, Type typeFrom, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        resolver.Resolve(typeFrom, null, dependencyOverrides, resolutionBehavior);

    /// <summary>
    /// Resolves a named instance from the container.
    /// </summary>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="typeFrom">The type of the requested service.</param>
    /// <param name="name">The name of the requested service.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static object Resolve(this IDependencyResolver resolver, Type typeFrom, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        resolver.Resolve(typeFrom, name, null, resolutionBehavior);
    
    /// <summary>
    /// Resolves a named instance from the container or returns default if the type is not resolvable.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="name">The name of the requested registration.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static TKey? ResolveOrDefault<TKey>(this IDependencyResolver resolver, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        (TKey?)(resolver.ResolveOrDefault(TypeCache<TKey>.Type, name, null, resolutionBehavior) ?? default(TKey));

    /// <summary>
    /// Resolves a named instance from the container with dependency overrides or returns default if the type is not resolvable.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="name">The name of the requested registration.</param>
    /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static TKey? ResolveOrDefault<TKey>(this IDependencyResolver resolver, object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        (TKey?)(resolver.ResolveOrDefault(TypeCache<TKey>.Type, name, dependencyOverrides, resolutionBehavior) ?? default(TKey));

    /// <summary>
    /// Resolves an instance from the container or returns default if the type is not resolvable.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <returns>The resolved object.</returns>
    public static TKey? ResolveOrDefault<TKey>(this IDependencyResolver resolver) =>
        (TKey?)(resolver.ResolveOrDefault(TypeCache<TKey>.Type) ?? default(TKey));
    
    /// <summary>
    /// Resolves an instance from the container or returns default if the type is not resolvable.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static TKey? ResolveOrDefault<TKey>(this IDependencyResolver resolver, ResolutionBehavior resolutionBehavior) =>
        (TKey?)(resolver.ResolveOrDefault(TypeCache<TKey>.Type, null, null, resolutionBehavior) ?? default(TKey));

    /// <summary>
    /// Resolves an instance from the container with dependency overrides or returns default if the type is not resolvable.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested instance.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static TKey? ResolveOrDefault<TKey>(this IDependencyResolver resolver, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        (TKey?)(resolver.ResolveOrDefault(TypeCache<TKey>.Type, null, dependencyOverrides, resolutionBehavior) ?? default(TKey));

    /// <summary>
    /// Resolves an instance from the container or returns default if the type is not resolvable.
    /// </summary>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="typeFrom">The type of the requested service.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static object? ResolveOrDefault(this IDependencyResolver resolver, Type typeFrom, ResolutionBehavior resolutionBehavior) =>
        resolver.ResolveOrDefault(typeFrom, null, null, resolutionBehavior);

    /// <summary>
    /// Resolves an instance from the container with dependency overrides or returns default if the type is not resolvable.
    /// </summary>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="typeFrom">The type of the requested service.</param>
    /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static object? ResolveOrDefault(this IDependencyResolver resolver, Type typeFrom, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        resolver.ResolveOrDefault(typeFrom, null, dependencyOverrides, resolutionBehavior);

    /// <summary>
    /// Resolves a named instance from the container or returns default if the type is not resolvable.
    /// </summary>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="typeFrom">The type of the requested service.</param>
    /// <param name="name">The name of the requested service.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static object? ResolveOrDefault(this IDependencyResolver resolver, Type typeFrom, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        resolver.ResolveOrDefault(typeFrom, name, null, resolutionBehavior);

    /// <summary>
    /// Resolves all registered implementations of a service.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested service.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <returns>The resolved object.</returns>
    public static IEnumerable<TKey> ResolveAll<TKey>(this IDependencyResolver resolver) =>
        (IEnumerable<TKey>)resolver.Resolve(TypeCache<IEnumerable<TKey>>.Type);
    
    /// <summary>
    /// Resolves all registered implementations of a service.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested service.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static IEnumerable<TKey> ResolveAll<TKey>(this IDependencyResolver resolver, ResolutionBehavior resolutionBehavior) =>
        (IEnumerable<TKey>)resolver.Resolve(TypeCache<IEnumerable<TKey>>.Type, null, null, resolutionBehavior);

    /// <summary>
    /// Resolves all registered implementations of a service identified by a name.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested service.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="name">The name of the requested service.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static IEnumerable<TKey> ResolveAll<TKey>(this IDependencyResolver resolver, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        (IEnumerable<TKey>)resolver.Resolve(TypeCache<IEnumerable<TKey>>.Type, name, null, resolutionBehavior);

    /// <summary>
    /// Resolves all registered implementations of a service with dependency overrides.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested service.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested services.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static IEnumerable<TKey> ResolveAll<TKey>(this IDependencyResolver resolver, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        (IEnumerable<TKey>)resolver.Resolve(TypeCache<IEnumerable<TKey>>.Type, null, dependencyOverrides, resolutionBehavior);

    /// <summary>
    /// Resolves all registered implementations of a service identified by a name and with dependency overrides.
    /// </summary>
    /// <typeparam name="TKey">The type of the requested services.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="name">The name of the requested services.</param>
    /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested services.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static IEnumerable<TKey> ResolveAll<TKey>(this IDependencyResolver resolver, object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        (IEnumerable<TKey>)resolver.Resolve(TypeCache<IEnumerable<TKey>>.Type, name, dependencyOverrides, resolutionBehavior);

    /// <summary>
    /// Resolves all registered implementations of a service.
    /// </summary>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="typeFrom">The type of the requested services.</param>
    /// <returns>The resolved object.</returns>
    public static IEnumerable<object> ResolveAll(this IDependencyResolver resolver, Type typeFrom)
    {
        var type = TypeCache.EnumerableType.MakeGenericType(typeFrom);
        return (IEnumerable<object>)resolver.Resolve(type);
    }
    
    /// <summary>
    /// Resolves all registered implementations of a service.
    /// </summary>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="typeFrom">The type of the requested services.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static IEnumerable<object> ResolveAll(this IDependencyResolver resolver, Type typeFrom,
        ResolutionBehavior resolutionBehavior)
    {
        var type = TypeCache.EnumerableType.MakeGenericType(typeFrom);
        return (IEnumerable<object>)resolver.Resolve(type, null, null, resolutionBehavior);
    }

    /// <summary>
    /// Resolves all registered implementations of a service.
    /// </summary>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="typeFrom">The type of the requested services.</param>
    /// <param name="name">The name of the requested services.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static IEnumerable<object> ResolveAll(this IDependencyResolver resolver, Type typeFrom, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        var type = TypeCache.EnumerableType.MakeGenericType(typeFrom);
        return (IEnumerable<object>)resolver.Resolve(type, name, null, resolutionBehavior);
    }

    /// <summary>
    /// Resolves all registered implementations of a service with dependency overrides.
    /// </summary>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="typeFrom">The type of the requested services.</param>
    /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested services.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static IEnumerable<object> ResolveAll(this IDependencyResolver resolver, Type typeFrom, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        var type = TypeCache.EnumerableType.MakeGenericType(typeFrom);
        return (IEnumerable<object>)resolver.Resolve(type, null, dependencyOverrides, resolutionBehavior);
    }

    /// <summary>
    /// Resolves all registered implementations of a service with dependency overrides.
    /// </summary>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="typeFrom">The type of the requested services.</param>
    /// <param name="name">The name of the requested services.</param>
    /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested services.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The resolved object.</returns>
    public static IEnumerable<object> ResolveAll(this IDependencyResolver resolver, Type typeFrom, object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        var type = TypeCache.EnumerableType.MakeGenericType(typeFrom);
        return (IEnumerable<object>)resolver.Resolve(type, name, dependencyOverrides, resolutionBehavior);
    }
    
    /// <summary>
    /// On the fly activates an object without registering it into the container. If you want to resolve a
    /// registered service use the <see cref="IDependencyResolver.Resolve(Type)" /> instead.
    /// </summary>
    /// <typeparam name="TTo">The service type.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="arguments">Optional dependency overrides.</param>
    /// <returns>The built object.</returns>
    public static TTo Activate<TTo>(this IDependencyResolver resolver, params object[] arguments) =>
        (TTo)resolver.Activate(TypeCache<TTo>.Type, arguments);

    /// <summary>
    /// Activates an object without registering it into the container. If you want to resolve a
    /// registered service use the <see cref="IDependencyResolver.Resolve(Type)" /> method instead.
    /// </summary>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="type">The type to activate.</param>
    /// <param name="arguments">Optional dependency overrides.</param>
    /// <returns>The built object.</returns>
    public static object Activate(this IDependencyResolver resolver, Type type, params object[] arguments) =>
        resolver.Activate(type, Constants.DefaultResolutionBehavior, arguments);
    
    /// <summary>
    /// On the fly activates an object without registering it into the container. If you want to resolve a
    /// registered service use the <see cref="IDependencyResolver.Resolve(Type)" /> instead.
    /// </summary>
    /// <typeparam name="TTo">The service type.</typeparam>
    /// <param name="resolver">The dependency resolver.</param>
    /// <param name="arguments">Optional dependency overrides.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>The built object.</returns>
    public static TTo Activate<TTo>(this IDependencyResolver resolver, ResolutionBehavior resolutionBehavior, params object[] arguments) =>
        (TTo)resolver.Activate(TypeCache<TTo>.Type, resolutionBehavior, arguments);

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
    
    /// <summary>
    /// Checks whether a type can be resolved by the container, or not.
    /// </summary>
    /// <typeparam name="TFrom">The service type.</typeparam>
    /// <param name="resolver">The resolver.</param>
    /// <param name="name">The registration name.</param>
    /// <param name="resolutionBehavior">The resolution behavior.</param>
    /// <returns>True if the service can be resolved, otherwise false.</returns>
    public static bool CanResolve<TFrom>(this IDependencyResolver resolver, object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        resolver.CanResolve(TypeCache<TFrom>.Type, name, resolutionBehavior);
}