using System;

namespace Stashbox.Resolution
{
    /// <summary>
    /// Represents an information storage for resolution requests.
    /// </summary>
    public interface IRequestContext
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

        /// <summary>
        /// Returns each dependency override passed to the resolution request.
        /// </summary>
        object[] GetOverrides();
    }

    internal interface IPerRequestInstanceHolder : IRequestContext
    {
        object GetOrAddInstance(int key, Func<IResolutionScope, IRequestContext, object> factory, IResolutionScope scope);
    }
}
