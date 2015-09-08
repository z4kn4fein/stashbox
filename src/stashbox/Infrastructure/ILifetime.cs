
using Stashbox.Entity;
namespace Stashbox.Infrastructure
{
    public interface ILifetime
    {
        object GetInstance(IObjectBuilder objectBuilder, IBuilderContext builderContext, ResolutionInfo resolutionInfo);
        void CleanUp();
    }
}
