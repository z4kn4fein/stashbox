using Stashbox.Entity;

namespace Stashbox.Infrastructure.ContainerExtension
{
    public interface IPreBuildExtension : IContainerExtension
    {
        void PreBuild(IBuilderContext builderContext, ResolutionInfo resolutionInfo);
    }
}
