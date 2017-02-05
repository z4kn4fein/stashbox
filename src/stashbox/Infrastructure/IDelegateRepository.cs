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
        /// Activates a type with a cached factory method.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <returns>The activated object.</returns>
        Func<object> GetDelegateCacheOrDefault(TypeInformation typeInfo);

        /// <summary>
        /// Adds a service delegate into the repository.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="factory">The factory delegate.</param>
        void AddServiceDelegate(TypeInformation typeInfo, Func<object> factory);

        /// <summary>
        /// Invalidates a service delegate in the repository.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        void InvalidateServiceDelegate(TypeInformation typeInfo);
    }
}
