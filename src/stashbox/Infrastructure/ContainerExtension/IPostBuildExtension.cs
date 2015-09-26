using Stashbox.Entity;
using System.Collections.Generic;

namespace Stashbox.Infrastructure.ContainerExtension
{
    public interface IPostBuildExtension : IContainerExtension
    {
        object PostBuild(object instance, IBuilderContext builderContext, ResolutionInfo resolutionInfo, HashSet<InjectionParameter> injectionParameters = null);
    }
}
