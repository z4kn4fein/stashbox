using System;
using Stashbox.Entity;

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
        /// <param name="typeInfo">The type info.</param>
        /// <returns>The cached factory delegate.</returns>
        Func<object> GetDelegateCacheOrDefault(TypeInformation typeInfo);

        /// <summary>
        /// Gets a cached factory method for a wrapper type.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <returns>The cached factory delegate.</returns>
        Func<object> GetWrapperDelegateCacheOrDefault(TypeInformation typeInfo);

        /// <summary>
        /// Gets a cached factory method.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="parameterType">The parameter type.</param>
        /// <returns>The cached factory delegate.</returns>
        Delegate GetFactoryDelegateCacheOrDefault(TypeInformation typeInfo, Type parameterType);

        /// <summary>
        /// Adds a service delegate into the repository.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="factory">The factory delegate.</param>
        void AddServiceDelegate(TypeInformation typeInfo, Func<object> factory);

        /// <summary>
        /// Adds a wrapper delegate into the repository.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="factory">The factory delegate.</param>
        void AddWrapperDelegate(WrappedDelegateInformation typeInfo, Func<object> factory);

        /// <summary>
        /// Adds a factory delegate into the repository.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="parameterType">The parameter type.</param>
        /// <param name="factory">The factory delegate.</param>
        void AddFactoryDelegate(TypeInformation typeInfo, Type parameterType, Delegate factory);

        /// <summary>
        /// Invalidates a service delegate in the repository.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        void InvalidateDelegateCache(TypeInformation typeInfo);
    }
}
