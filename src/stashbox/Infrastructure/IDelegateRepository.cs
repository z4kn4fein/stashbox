using System;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a delegate factory.
    /// </summary>
    public interface IDelegateRepository
    {
        /// <summary>
        /// Gets a cached factory method for a type.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <returns>The cached factory delegate.</returns>
        Func<IResolutionScope, object> GetDelegateCacheOrDefault(Type type);

        /// <summary>
        /// Gets a cached factory method for a type.
        /// </summary>
        /// <param name="name">The service name.</param>
        /// <returns>The cached factory delegate.</returns>
        Func<IResolutionScope, object> GetDelegateCacheOrDefault(object name);

        /// <summary>
        /// Gets a cached factory method.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="name">The service name.</param>
        /// <returns>The cached factory delegate.</returns>
        Func<IResolutionScope, Delegate> GetFactoryDelegateCacheOrDefault(Type type, object name = null);

        /// <summary>
        /// Adds a service delegate into the repository.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="name">The service name.</param>
        void AddServiceDelegate(Type type, Func<IResolutionScope, object> factory, object name = null);

        /// <summary>
        /// Adds a factory delegate into the repository.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="name">The service name.</param>
        void AddFactoryDelegate(Type type, Func<IResolutionScope, Delegate> factory, object name = null);

        /// <summary>
        /// Invalidates a service delegate in the repository.
        /// </summary>
        void InvalidateDelegateCache();
    }
}
