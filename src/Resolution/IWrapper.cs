using System;
using System.Collections.Generic;

namespace Stashbox.Resolution
{
    /// <summary>
    /// Represents a service that unwraps the underlying service types from a wrapped type request.
    /// </summary>
    public interface IWrapper
    {
        /// <summary>
        /// Unwraps the underlying service types from a wrapped type request.
        /// </summary>
        /// <param name="typeInfo">The requested type's meta information.</param>
        /// <param name="unWrappedTypes">The wrapped service types.</param>
        /// <returns>True if the unwrapping was successful, otherwise false.</returns>
        bool TryUnWrap(TypeInformation typeInfo, out IEnumerable<Type> unWrappedTypes);
    }
}
