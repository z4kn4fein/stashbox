using Stashbox.Entity;

namespace Stashbox.Infrastructure.ContainerExtension
{
    public interface IPreBuildExtension : IContainerExtension
    {
        void PreBuild(IContainerContext containerContext, ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null);
    }
}
