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
        /// <param name="name">The service name.</param>
        /// <returns>The cached factory delegate.</returns>
        Func<IResolutionScope, object> GetDelegateCacheOrDefault(Type type, string name = null);

        /// <summary>
        /// Gets a cached factory method.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <param name="name">The service name.</param>
        /// <returns>The cached factory delegate.</returns>
        Func<IResolutionScope, Delegate> GetFactoryDelegateCacheOrDefault(Type type, Type[] parameterTypes, string name = null);

        /// <summary>
        /// Adds a service delegate into the repository.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="name">The service name.</param>
        void AddServiceDelegate(Type type, Func<IResolutionScope, object> factory, string name = null);

        /// <summary>
        /// Adds a factory delegate into the repository.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="parameterTypes">The parameter type.</param>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="name">The service name.</param>
        void AddFactoryDelegate(Type type, Type[] parameterTypes, Func<IResolutionScope, Delegate> factory, string name = null);

        /// <summary>
        /// Invalidates a service delegate in the repository.
        /// </summary>
        void InvalidateDelegateCache();
    }
}
