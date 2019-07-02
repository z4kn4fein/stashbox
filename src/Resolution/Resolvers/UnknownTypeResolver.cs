using Stashbox.Entity;
using System;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class UnknownTypeResolver : IResolver
    {
        public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            containerContext.ContainerConfigurator.ContainerConfiguration.UnknownTypeResolutionEnabled &&
                       typeInfo.Type.IsResolvableType();

        public Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            containerContext.Container.Register(typeInfo.Type,
                containerContext.ContainerConfigurator.ContainerConfiguration.UnknownTypeConfigurator);
            return containerContext.ResolutionStrategy.BuildResolutionExpression(containerContext, resolutionContext, typeInfo);
        }
    }
}
