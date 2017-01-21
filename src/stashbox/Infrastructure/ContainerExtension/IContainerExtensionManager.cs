using Stashbox.Entity;

namespace Stashbox.Infrastructure.ContainerExtension
{
    internal interface IContainerExtensionManager
    {
        void AddExtension(IContainerExtension containerExtension);
        object ExecutePostBuildExtensions(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo,
            TypeInformation resolveType, InjectionParameter[] injectionParameters = null);
        void ExecuteOnRegistrationExtensions(IContainerContext containerContext, RegistrationInfo registrationInfo, InjectionParameter[] injectionParameters = null);
        IContainerExtensionManager CreateCopy();
        void ReinitalizeExtensions(IContainerContext containerContext);
        bool HasPostBuildExtensions { get; }
        void CleanUp();
    }
}
