using System;

namespace Stashbox.Resolution
{
    /// <summary>
    /// Represents the public context of a resolution call.
    /// </summary>
    public interface IResolutionContext
    {
        /// <summary>
        /// Returns a dependency override for a given type.
        /// </summary>
        /// <param name="dependencyType">The type of the dependency override.</param>
        /// <returns>The object used for override a dependency.</returns>
        object GetDependencyOverrideOrDefault(Type dependencyType);

        /// <summary>
        /// Returns a dependency override for a given type.
        /// </summary>
        /// <typeparam name="TResult">The type of the dependency override.</typeparam>
        /// <returns>The object used for override a dependency.</returns>
        TResult GetDependencyOverrideOrDefault<TResult>();
    }
}