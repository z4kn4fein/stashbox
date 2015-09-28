using Stashbox.Entity;
using System.Collections.Generic;

namespace Stashbox.Infrastructure.ContainerExtension
{
    public interface IPreBuildExtension : IContainerExtension
    {
        void PreBuild(IContainerContext containerContext, ResolutionInfo resolutionInfo, HashSet<InjectionParameter> injectionParameters = null);
    }
}
