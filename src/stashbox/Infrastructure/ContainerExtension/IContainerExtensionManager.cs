using Stashbox.Infrastructure.Registration;
using Stashbox.Resolution;
using System;

namespace Stashbox.Infrastructure.ContainerExtension
{
    internal interface IContainerExtensionManager
    {
        void AddExtension(IContainerExtension containerExtension);
        object ExecutePostBuildExtensions(object instance, IContainerContext containerContext, ResolutionContext resolutionContext,
            IServiceRegistration serviceRegistration, Type requestedType);
        void ExecuteOnRegistrationExtensions(IContainerContext containerContext, IServiceRegistration serviceRegistration);
        IContainerExtensionManager CreateCopy();
        void ReinitalizeExtensions(IContainerContext containerContext);
        bool HasPostBuildExtensions { get; }
        bool HasRegistrationExtensions { get; }
        void CleanUp();
    }
}
