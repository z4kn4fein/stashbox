using Stashbox.Entity;

namespace Stashbox.Infrastructure
{
    internal interface IBuildExtensionManager
    {
        void AddExtension(BuildExtension buildExtension);
        object ExecutePostBuildExtensions(object instance, IBuilderContext builderContext, ResolutionInfo resolutionInfo);
        void ExecutePreBuildExtensions(IBuilderContext builderContext, ResolutionInfo resolutionInfo);
        void ExecuteOnRegistrationExtensions(IBuilderContext builderContext, RegistrationInfo registrationInfo);
    }
}
