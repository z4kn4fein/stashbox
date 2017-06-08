using System;
using Stashbox.Entity;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.Infrastructure.ContainerExtension
{
    internal interface IContainerExtensionManager
    {
        void AddExtension(IContainerExtension containerExtension);
        object ExecutePostBuildExtensions(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo,
            IServiceRegistration serviceRegistration, Type requestedType);
        void ExecuteOnRegistrationExtensions(IContainerContext containerContext, IServiceRegistration serviceRegistration);
        IContainerExtensionManager CreateCopy();
        void ReinitalizeExtensions(IContainerContext containerContext);
        bool HasPostBuildExtensions { get; }
        bool HasRegistrationExtensions { get; }
        void CleanUp();
    }
}
