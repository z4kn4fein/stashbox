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
        object ActivateFromDelegateCacheOrDefault(TypeInformation typeInfo);

        /// <summary>
        /// Adds a service delegate into the repository.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="factory">The factory delegate.</param>
        void AddServiceDelegate(TypeInformation typeInfo, Func<object> factory);

        /// <summary>
        /// Adds a factory delegate which gets the <see cref="IStashboxContainer"/> as a parameter into the repository.
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="factory"></param>
        void AddContainereDelegate(TypeInformation typeInfo, Func<IStashboxContainer, object> factory);
    }
}
