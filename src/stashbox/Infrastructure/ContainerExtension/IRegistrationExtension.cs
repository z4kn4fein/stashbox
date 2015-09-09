using Stashbox.Entity;

namespace Stashbox.Infrastructure.ContainerExtension
{
    public interface IRegistrationExtension : IContainerExtension
    {
        void OnRegistration(IBuilderContext builderContext, RegistrationInfo registrationInfo);
    }
}
