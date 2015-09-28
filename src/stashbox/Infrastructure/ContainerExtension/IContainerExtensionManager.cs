using Stashbox.Entity;
using System.Collections.Generic;

namespace Stashbox.Infrastructure.ContainerExtension
{
    internal interface IContainerExtensionManager
    {
        void AddExtension(IContainerExtension containerExtension);
        object ExecutePostBuildExtensions(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo, HashSet<InjectionParameter> injectionParameters = null);
        void ExecutePreBuildExtensions(IContainerContext containerContext, ResolutionInfo resolutionInfo, HashSet<InjectionParameter> injectionParameters = null);
        void ExecuteOnRegistrationExtensions(IContainerContext containerContext, RegistrationInfo registrationInfo, HashSet<InjectionParameter> injectionParameters = null);
    }
}
