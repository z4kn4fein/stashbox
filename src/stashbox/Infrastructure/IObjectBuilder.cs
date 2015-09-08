
using Stashbox.Entity;
namespace Stashbox.Infrastructure
{
    public interface IObjectBuilder
    {
        object BuildInstance(IBuilderContext builderContext, ResolutionInfo resolutionInfo);
        void CleanUp();
    }
}
