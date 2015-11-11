using Stashbox.Entity;
using System;
using System.Collections.Generic;

namespace Stashbox.Infrastructure.ContainerExtension
{
    internal interface IContainerExtensionManager
    {
        void AddExtension(IContainerExtension containerExtension);
        object ExecutePostBuildExtensions(object instance, Type targetType, IContainerContext containerContext, ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null);
        void ExecuteOnRegistrationExtensions(IContainerContext containerContext, RegistrationInfo registrationInfo, InjectionParameter[] injectionParameters = null);
    }
}
