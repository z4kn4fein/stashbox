using Stashbox.Entity;
using System;

namespace Stashbox.Infrastructure.ContainerExtension
{
    internal interface IContainerExtensionManager
    {
        void AddExtension(IContainerExtension containerExtension);
        object ExecutePostBuildExtensions(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo,
            Type resolveType, InjectionParameter[] injectionParameters = null);
        void ExecuteOnRegistrationExtensions(IContainerContext containerContext, Type typeTo, Type typeFrom, InjectionParameter[] injectionParameters = null);
        IContainerExtensionManager CreateCopy();
        void ReinitalizeExtensions(IContainerContext containerContext);
        bool HasPostBuildExtensions { get; }
        void CleanUp();
    }
}
