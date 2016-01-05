using Stashbox.Entity;
using System;

namespace Stashbox.Infrastructure.ContainerExtension
{
    public interface IPostBuildExtension : IContainerExtension
    {
        object PostBuild(object instance, Type targetType, IContainerContext containerContext, ResolutionInfo resolutionInfo,
            TypeInformation resolveType, InjectionParameter[] injectionParameters = null);
    }
}
