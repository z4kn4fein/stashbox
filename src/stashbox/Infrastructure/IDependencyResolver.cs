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
        /// <param name="factoryParameters">The parameters for the registered factory delegate.</param>
        /// <param name="overrides">Parameter overrides.</param>
        /// <returns>The resolved object.</returns>
        TKey Resolve<TKey>(string name = null, IEnumerable<object> factoryParameters = null, IEnumerable<Override> overrides = null)
           where TKey : class;

        /// <summary>
        /// Resolves an instance from the container.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instance.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="factoryParameters">The parameters for the registered factory delegate.</param>
        /// <param name="overrides">Parameter overrides.</param>
        /// <returns>The resolved object.</returns>
        object Resolve(Type typeFrom, string name = null, IEnumerable<object> factoryParameters = null, IEnumerable<Override> overrides = null);

        /// <summary>
        /// Resolves all registered types of a service.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested instance.</typeparam>
        /// <param name="factoryParameters">The parameters for the registered factory delegate.</param>
        /// <param name="overrides">Parameter overrides.</param>
        /// <returns>The resolved object.</returns>
        IEnumerable<TKey> ResolveAll<TKey>(IEnumerable<object> factoryParameters = null, IEnumerable<Override> overrides = null)
             where TKey : class;
    }
}
