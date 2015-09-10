using Stashbox.Entity;

namespace Stashbox.Infrastructure.ContainerExtension
{
    internal interface IContainerExtensionManager
    {
        void AddExtension(IContainerExtension containerExtension);
        object ExecutePostBuildExtensions(object instance, IBuilderContext builderContext, ResolutionInfo resolutionInfo);
        void ExecutePreBuildExtensions(IBuilderContext builderContext, ResolutionInfo resolutionInfo);
        void ExecuteOnRegistrationExtensions(IBuilderContext builderContext, RegistrationInfo registrationInfo);
    }
}
