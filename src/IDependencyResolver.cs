using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox
{
    /// <summary>
    /// Represents a dependency resolver.
    /// </summary>
    public interface IDependencyResolver : IServiceProvider,
#if HAS_ASYNC_DISPOSABLE
        IAsyncDisposable,
#endif
        IDisposable
    {
        /// <summary>
        /// Resolves an instance from the container.
        /// </summary>
        /// <param name="typeFrom">The type of the requested service.</param>
        /// <returns>The resolved object.</returns>
        object Resolve(Type typeFrom);

        /// <summary>
        /// Resolves an instance from the container with dependency overrides.
        /// </summary>
        /// <param name="typeFrom">The type of the requested service.</param>
        /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
        /// <returns>The resolved object.</returns>
        object Resolve(Type typeFrom, object[] dependencyOverrides);

        /// <summary>
        /// Resolves a named instance from the container.
        /// </summary>
        /// <param name="typeFrom">The type of the requested service.</param>
        /// <param name="name">The name of the requested service.</param>
        /// <returns>The resolved object.</returns>
        object Resolve(Type typeFrom, object name);

        /// <summary>
        /// Resolves a named instance from the container with dependency overrides.
        /// </summary>
        /// <param name="typeFrom">The type of the requested service.</param>
        /// <param name="name">The name of the requested service.</param>
        /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
        /// <returns>The resolved object.</returns>
        object Resolve(Type typeFrom, object name, object[] dependencyOverrides);

        /// <summary>
        /// Resolves an instance from the container or returns default if the type is not resolvable.
        /// </summary>
        /// <param name="typeFrom">The type of the requested service.</param>
        /// <returns>The resolved object.</returns>
        object? ResolveOrDefault(Type typeFrom);

        /// <summary>
        /// Resolves an instance from the container with dependency overrides or returns default if the type is not resolvable.
        /// </summary>
        /// <param name="typeFrom">The type of the requested service.</param>
        /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
        /// <returns>The resolved object.</returns>
        object? ResolveOrDefault(Type typeFrom, object[] dependencyOverrides);

        /// <summary>
        /// Resolves a named instance from the container or returns default if the type is not resolvable.
        /// </summary>
        /// <param name="typeFrom">The type of the requested service.</param>
        /// <param name="name">The name of the requested service.</param>
        /// <returns>The resolved object.</returns>
        object? ResolveOrDefault(Type typeFrom, object name);

        /// <summary>
        /// Resolves an instance from the container with dependency overrides or returns default if the type is not resolvable.
        /// </summary>
        /// <param name="typeFrom">The type of the requested service.</param>
        /// <param name="name">The name of the requested service.</param>
        /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
        /// <returns>The resolved object.</returns>
        object? ResolveOrDefault(Type typeFrom, object name, object[] dependencyOverrides);

        /// <summary>
        /// Resolves all registered implementations of a service.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested service.</typeparam>
        /// <returns>The resolved object.</returns>
        IEnumerable<TKey> ResolveAll<TKey>();

        /// <summary>
        /// Resolves all registered implementations of a service identified by a name.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested service.</typeparam>
        /// <returns>The resolved object.</returns>
        IEnumerable<TKey> ResolveAll<TKey>(object name);

        /// <summary>
        /// Resolves all registered implementations of a service with dependency overrides.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested service.</typeparam>
        /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested services.</param>
        /// <returns>The resolved object.</returns>
        IEnumerable<TKey> ResolveAll<TKey>(object[] dependencyOverrides);

        /// <summary>
        /// Resolves all registered implementations of a service identified by a name and with dependency overrides.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested services.</typeparam>
        /// <param name="name">The name of the requested services.</param>
        /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested services.</param>
        /// <returns>The resolved object.</returns>
        IEnumerable<TKey> ResolveAll<TKey>(object name, object[] dependencyOverrides);

        /// <summary>
        /// Resolves all registered implementations of a service.
        /// </summary>
        /// <param name="typeFrom">The type of the requested services.</param>
        /// <returns>The resolved object.</returns>
        IEnumerable<object> ResolveAll(Type typeFrom);

        /// <summary>
        /// Resolves all registered implementations of a service.
        /// </summary>
        /// <param name="typeFrom">The type of the requested services.</param>
        /// <param name="name">The name of the requested services.</param>
        /// <returns>The resolved object.</returns>
        IEnumerable<object> ResolveAll(Type typeFrom, object name);

        /// <summary>
        /// Resolves all registered implementations of a service with dependency overrides.
        /// </summary>
        /// <param name="typeFrom">The type of the requested services.</param>
        /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested services.</param>
        /// <returns>The resolved object.</returns>
        IEnumerable<object> ResolveAll(Type typeFrom, object[] dependencyOverrides);

        /// <summary>
        /// Resolves all registered implementations of a service with dependency overrides.
        /// </summary>
        /// <param name="typeFrom">The type of the requested services.</param>
        /// <param name="name">The name of the requested services.</param>
        /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested services.</param>
        /// <returns>The resolved object.</returns>
        IEnumerable<object> ResolveAll(Type typeFrom, object name, object[] dependencyOverrides);

        /// <summary>
        /// Returns a factory delegate that can be used to activate the service.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instances.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="parameterTypes">The parameter type.</param>
        /// <returns>The factory delegate.</returns>
        Delegate ResolveFactory(Type typeFrom, object? name = null, params Type[] parameterTypes);

        /// <summary>
        /// Returns a factory delegate that can be used to activate the service or returns default if the type is not resolvable.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instances.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="parameterTypes">The parameter type.</param>
        /// <returns>The factory delegate.</returns>
        Delegate? ResolveFactoryOrDefault(Type typeFrom, object? name = null, params Type[] parameterTypes);

        /// <summary>
        /// Creates a new scope.
        /// </summary>
        /// <param name="name">The name of the scope.</param>
        /// <param name="attachToParent">If true, the new scope will be attached to the lifecycle of its parent scope. When the parent is being disposed, the new scope will be disposed with it.</param>
        /// <returns>The created scope.</returns>
        IDependencyResolver BeginScope(object? name = null, bool attachToParent = false);

        /// <summary>
        /// Puts an instance into the scope. The instance will be disposed along with the scope disposal.
        /// </summary>
        /// <param name="typeFrom">The service type.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from disposal tracking.</param>
        /// <param name="name">The dependency name of the instance.</param>
        /// <returns>The scope.</returns>
        void PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false, object? name = null);

        /// <summary>
        /// Builds up an existing instance. This means the container performs member and method injections on it without registering it into the container.
        /// </summary>
        /// <typeparam name="TTo">The type of the requested instance.</typeparam>
        /// <param name="instance">The instance to build up.</param>
        /// <returns>The built object.</returns>
        TTo BuildUp<TTo>(TTo instance)
            where TTo : class;

        /// <summary>
        /// Activates an object without registering it into the container. If you want to resolve a
        /// registered service use the <see cref="Resolve(Type, object[])" /> method instead.
        /// </summary>
        /// <param name="type">The type to activate.</param>
        /// <param name="arguments">Optional dependency overrides.</param>
        /// <returns>The built object.</returns>
        object Activate(Type type, params object[] arguments);

        /// <summary>
        /// Calls the registered asynchronous initializers of all resolved objects.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The initializer task.</returns>
        ValueTask InvokeAsyncInitializers(CancellationToken token = default);

        /// <summary>
        /// Checks whether a type can be resolved by the container, or not.
        /// </summary>
        /// <typeparam name="TFrom">The service type.</typeparam>
        /// <param name="name">The registration name.</param>
        /// <returns>True if the service can be resolved, otherwise false.</returns>
        bool CanResolve<TFrom>(object? name = null);

        /// <summary>
        /// Checks whether a type can be resolved by the container, or not.
        /// </summary>
        /// <param name="typeFrom">The service type.</param>
        /// <param name="name">The registration name.</param>
        /// <returns>True if the service can be resolved, otherwise false.</returns>
        bool CanResolve(Type typeFrom, object? name = null);

        /// <summary>
        /// Returns all cached service resolution delegates.
        /// </summary>
        /// <returns>The service resolution delegates.</returns>
        IEnumerable<DelegateCacheEntry> GetDelegateCacheEntries();
    }
}
