using Stashbox.Overrides;
using System;
using System.Collections.Generic;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a dependency resolver.
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// Resolves an instance from the container.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested instance.</typeparam>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="overrides">Parameter overrides.</param>
        /// <returns>The resolved object.</returns>
        TKey Resolve<TKey>(string name = null, params TypeOverride[] overrides)
           where TKey : class;

        /// <summary>
        /// Resolves an instance from the container.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instance.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="overrides">Parameter overrides.</param>
        /// <returns>The resolved object.</returns>
        object Resolve(Type typeFrom, string name = null, params TypeOverride[] overrides);

        /// <summary>
        /// Resolves all registered types of a service.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested instance.</typeparam>
        /// <param name="overrides">Parameter overrides.</param>
        /// <returns>The resolved object.</returns>
        IEnumerable<TKey> ResolveAll<TKey>(params TypeOverride[] overrides)
             where TKey : class;

        /// <summary>
        /// Resolves all registered types of a service.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instances.</param>
        /// <param name="overrides">Parameter overrides.</param>
        /// <returns>The resolved object.</returns>
        IEnumerable<object> ResolveAll(Type typeFrom, params TypeOverride[] overrides);

        /// <summary>
        /// Builds up an instance, the container will perform injections and extensions on it.
        /// </summary>
        /// <typeparam name="TTo">The type of the requested instance.</typeparam>
        /// <param name="instance">The instance to build up.</param>
        /// <returns>The built object.</returns>
        TTo BuildUp<TTo>(TTo instance);
    }
}
