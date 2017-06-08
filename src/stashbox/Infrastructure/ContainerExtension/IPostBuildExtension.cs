using System;
using Stashbox.Entity;
using Stashbox.Infrastructure.Registration;

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
        /// <param name="serviceRegistration">The service registration.</param>
        /// <param name="requestedType">The requested type.</param>
        /// <returns>The extended object.</returns>
        object PostBuild(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo,
            IServiceRegistration serviceRegistration, Type requestedType);
    }
}
