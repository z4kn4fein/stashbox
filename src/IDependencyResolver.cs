using Stashbox.Exceptions;
using System;
using System.Collections.Generic;

namespace Stashbox
{
    /// <summary>
    /// Represents a dependency resolver.
    /// </summary>
    public interface IDependencyResolver :
#if HAS_SERVICEPROVIDER
        IServiceProvider,
#endif
        IDisposable
    {
        /// <summary>
        /// Resolves an instance from the container.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instance.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
        /// <returns>The resolved object.</returns>
        object Resolve(Type typeFrom, bool nullResultAllowed = false, object[] dependencyOverrides = null);

        /// <summary>
        /// Resolves an instance from the container.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instance.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
        /// <returns>The resolved object.</returns>
        object Resolve(Type typeFrom, object name, bool nullResultAllowed = false, object[] dependencyOverrides = null);

        /// <summary>
        /// Resolves all registered types of a service.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested instance.</typeparam>
        /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested services.</param>
        /// <returns>The resolved object.</returns>
        IEnumerable<TKey> ResolveAll<TKey>(object[] dependencyOverrides = null);

        /// <summary>
        /// Resolves all registered types of a service.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instances.</param>
        /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested services.</param>
        /// <returns>The resolved object.</returns>
        IEnumerable<object> ResolveAll(Type typeFrom, object[] dependencyOverrides = null);

        /// <summary>
        /// Returns a factory method which can be used to activate a type.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instances.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <param name="parameterTypes">The parameter type.</param>
        /// <returns>The factory delegate.</returns>
        Delegate ResolveFactory(Type typeFrom, object name = null, bool nullResultAllowed = false, params Type[] parameterTypes);

        /// <summary>
        /// Begins a new scope.
        /// </summary>
        /// <param name="name">The name of the scope.</param>
        /// <param name="attachToParent">If true, the new scope will be attached to the lifecycle of the parent scope, when it's disposed, the new scope will be disposed with it.</param>
        /// <returns>The created scope.</returns>
        IDependencyResolver BeginScope(object name = null, bool attachToParent = false);

        /// <summary>
        /// Puts an instance into the scope which will be dropped when the scope is being disposed.
        /// </summary>
        /// <param name="typeFrom">The service type.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from the disposal tracking.</param>
        /// <returns>The scope.</returns>
        IDependencyResolver PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false);

        /// <summary>
        /// Builds up an instance, the container will perform injections and extensions on it.
        /// </summary>
        /// <typeparam name="TTo">The type of the requested instance.</typeparam>
        /// <param name="instance">The instance to build up.</param>
        /// <returns>The built object.</returns>
        TTo BuildUp<TTo>(TTo instance);

        /// <summary>
        /// On the fly activates an object without registering it into the container. If you want to resolve a
        /// registered service use the <see cref="IDependencyResolver.Resolve(Type, bool, object[])" /> instead.
        /// </summary>
        /// <param name="type">The type to activate.</param>
        /// <param name="arguments">Optional dependency overrides.</param>
        /// <returns>The built object.</returns>
        object Activate(Type type, params object[] arguments);
    }
}
