using System;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class UnknownTypeResolver : Resolver
    {
        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo) =>
            containerContext.ContainerConfigurator.ContainerConfiguration.UnknownTypeResolutionEnabled &&
                       typeInfo.Type.IsValidForRegistration();

        public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
        {
            containerContext.Container.RegisterType(typeInfo.Type, typeInfo.Type, context => context.WithName(typeInfo.DependencyName));
            return containerContext.ResolutionStrategy.BuildResolutionExpression(containerContext, resolutionInfo, typeInfo, null);
        }
    }
}
