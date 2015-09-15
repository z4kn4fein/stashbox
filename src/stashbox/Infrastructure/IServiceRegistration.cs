using Stashbox.Entity;
namespace Stashbox.Infrastructure
{
    public interface IServiceRegistration
    {
        RegistrationInfo RegistrationInfo { get; }
        object GetInstance(ResolutionInfo resolutionInfo);
        void CleanUp();
    }
}
