
using Stashbox.Entity;
namespace Stashbox.Infrastructure
{
    public interface IObjectBuilder
    {
        object BuildInstance(IContainerContext containerContext, ResolutionInfo resolutionInfo);
        void CleanUp();
    }
}
