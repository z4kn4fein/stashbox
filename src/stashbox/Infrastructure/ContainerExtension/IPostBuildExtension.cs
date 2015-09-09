using Stashbox.Entity;

namespace Stashbox.Infrastructure.ContainerExtension
{
    public interface IPostBuildExtension : IContainerExtension
    {
        object PostBuild(object instance, IBuilderContext builderContext, ResolutionInfo resolutionInfo);
    }
}
