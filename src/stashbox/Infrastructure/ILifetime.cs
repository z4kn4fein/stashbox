
using Stashbox.Entity;
namespace Stashbox.Infrastructure
{
    public interface ILifetime
    {
        object GetInstance(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo);
        void CleanUp();
    }
}
