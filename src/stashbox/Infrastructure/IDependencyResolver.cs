using Stashbox.Exceptions;
using System;
using System.Collections.Generic;
using Stashbox.Infrastructure.Resolution;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a dependency resolver.
    /// </summary>
    public interface IDependencyResolver : IDisposable
    {
        /// <summary>
        /// The activation context.
        /// </summary>
        IActivationContext ActivationContext { get; }

        /// <summary>
        /// Resolves an instance from the container.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested instance.</typeparam>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The resolved object.</returns>
        TKey Resolve<TKey>(string name = null, bool nullResultAllowed = false)
           where TKey : class;

        /// <summary>
        /// Resolves an instance from the container.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instance.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The resolved object.</returns>
        object Resolve(Type typeFrom, string name = null, bool nullResultAllowed = false);

        /// <summary>
        /// Resolves all registered types of a service.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested instance.</typeparam>
        /// <returns>The resolved object.</returns>
        IEnumerable<TKey> ResolveAll<TKey>()
             where TKey : class;

        /// <summary>
        /// Resolves all registered types of a service.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instances.</param>
        /// <returns>The resolved object.</returns>
        IEnumerable<object> ResolveAll(Type typeFrom);

        /// <summary>
        /// Returns with a factory method which can be used to activate a type.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instances.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <param name="parameterTypes">The parameter type.</param>
        /// <returns>The factory delegate.</returns>
        Delegate ResolveFactory(Type typeFrom, string name = null, bool nullResultAllowed = false, params Type[] parameterTypes);
        
        /// <summary>
        /// Begins a new scope.
        /// </summary>
        IDependencyResolver BeginScope();
    }
}
