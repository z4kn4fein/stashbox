using Stashbox.Entity;
using System;
using System.Collections.Generic;

namespace Stashbox.Infrastructure.ContainerExtension
{
    public interface IPostBuildExtension : IContainerExtension
    {
        object PostBuild(object instance, Type targetType, IContainerContext containerContext, ResolutionInfo resolutionInfo, HashSet<InjectionParameter> injectionParameters = null);
    }
}
