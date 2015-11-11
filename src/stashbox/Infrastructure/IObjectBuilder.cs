
using Stashbox.Entity;
namespace Stashbox.Infrastructure
{
    public interface IObjectBuilder
    {
        object BuildInstance(ResolutionInfo resolutionInfo);
        void CleanUp();
    }
}
