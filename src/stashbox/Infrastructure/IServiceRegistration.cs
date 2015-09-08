using Stashbox.Entity;
namespace Stashbox.Infrastructure
{
    public interface IServiceRegistration
    {
        object GetInstance(ResolutionInfo resolutionInfo);
        void CleanUp();
    }
}
