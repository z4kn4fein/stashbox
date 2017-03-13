using System;
using Stashbox.Entity;

namespace Stashbox.Infrastructure.ContainerExtension
{
    /// <summary>
    /// Represents a post build extension.
    /// </summary>
    public interface IPostBuildExtension : IContainerExtension
    {
        /// <summary>
        /// Executes the post build extension.
        /// </summary>
        /// <param name="instance">The resolved object.</param>
        /// <param name="containerContext">The <see cref="IContainerContext"/> of the <see cref="StashboxContainer"/></param>
        /// <param name="resolutionInfo">Information about the actual resolution.</param>
        /// <param name="resolveType">The type information of the resolved type.</param>
        /// <param name="injectionParameters">The injection parameters.</param>
        /// <returns>The extended object.</returns>
        object PostBuild(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo,
            Type resolveType, InjectionParameter[] injectionParameters = null);
    }
}
