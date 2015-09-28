
using Stashbox.Entity;
namespace Stashbox.Infrastructure
{
    public interface ILifetime
    {
        object GetInstance(IObjectBuilder objectBuilder, IContainerContext containerContext, ResolutionInfo resolutionInfo);
        void CleanUp();
    }
}
