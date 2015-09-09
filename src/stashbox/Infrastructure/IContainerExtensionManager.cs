using Stashbox.Entity;
using Stashbox.Infrastructure.ContainerExtension;

namespace Stashbox.Infrastructure
{
    internal interface IContainerExtensionManager
    {
        void AddExtension(IContainerExtension containerExtension);
        object ExecutePostBuildExtensions(object instance, IBuilderContext builderContext, ResolutionInfo resolutionInfo);
        void ExecutePreBuildExtensions(IBuilderContext builderContext, ResolutionInfo resolutionInfo);
        void ExecuteOnRegistrationExtensions(IBuilderContext builderContext, RegistrationInfo registrationInfo);
    }
}
