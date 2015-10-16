using Stashbox.Entity;
namespace Stashbox.Infrastructure
{
    public interface IServiceRegistration
    {
        object GetInstance(ResolutionInfo resolutionInfo);
        bool IsUsableForCurrentContext(TypeInformation resolutionInfo);
        bool HasCondition { get; }
        void CleanUp();
    }
}
