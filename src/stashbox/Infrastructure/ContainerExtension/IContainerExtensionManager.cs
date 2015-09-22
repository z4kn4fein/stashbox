using Stashbox.Entity;
using System.Collections.Generic;

namespace Stashbox.Infrastructure.ContainerExtension
{
    internal interface IContainerExtensionManager
    {
        void AddExtension(IContainerExtension containerExtension);
        object ExecutePostBuildExtensions(object instance, IBuilderContext builderContext, ResolutionInfo resolutionInfo, HashSet<InjectionParameter> injectionParameters = null);
        void ExecutePreBuildExtensions(IBuilderContext builderContext, ResolutionInfo resolutionInfo, HashSet<InjectionParameter> injectionParameters = null);
        void ExecuteOnRegistrationExtensions(IBuilderContext builderContext, RegistrationInfo registrationInfo, HashSet<InjectionParameter> injectionParameters = null);
    }
}
