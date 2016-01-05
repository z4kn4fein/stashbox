using Stashbox.Entity;
using System;

namespace Stashbox.Infrastructure.ContainerExtension
{
    internal interface IContainerExtensionManager
    {
        void AddExtension(IContainerExtension containerExtension);
        object ExecutePostBuildExtensions(object instance, Type targetType, IContainerContext containerContext, ResolutionInfo resolutionInfo,
            TypeInformation resolveType, InjectionParameter[] injectionParameters = null);
        void ExecuteOnRegistrationExtensions(IContainerContext containerContext, RegistrationInfo registrationInfo, InjectionParameter[] injectionParameters = null);
        IContainerExtensionManager CreateCopy();
        void ReinitalizeExtensions(IContainerContext containerContext);
        bool HasPostBuildExtensions { get; }
        void CleanUp();
    }
}
