using Stashbox.Entity;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;
using Stashbox.Registration;

namespace Stashbox.BuildUp.Resolution
{
    internal class UnknownTypeResolver : IResolver
    {
        public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            containerContext.ContainerConfigurator.ContainerConfiguration.UnknownTypeResolutionEnabled &&
                       typeInfo.Type.IsValidForRegistration();

        public Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            containerContext.Container.Register(typeInfo.Type,
                containerContext.ContainerConfigurator.ContainerConfiguration.UnknownTypeConfigurator);
            return containerContext.ResolutionStrategy.BuildResolutionExpression(containerContext, resolutionContext, typeInfo, null);
        }
    }
}
