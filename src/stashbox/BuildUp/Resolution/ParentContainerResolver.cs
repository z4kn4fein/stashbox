using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class ParentContainerResolver : Resolver
    {
        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo) =>
            containerContext.Container.ParentContainer != null && containerContext.Container.ParentContainer.CanResolve(typeInfo.Type, typeInfo.DependencyName);

        public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo) =>
            containerContext.Container.ParentContainer.ContainerContext.ResolutionStrategy
                .BuildResolutionExpression(containerContext.Container.ParentContainer.ContainerContext, resolutionInfo, typeInfo, null);

        public override Expression[] GetExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo) =>
            containerContext.Container.ParentContainer.ContainerContext.ResolutionStrategy
                .BuildResolutionExpressions(containerContext.Container.ParentContainer.ContainerContext, resolutionInfo, typeInfo);
    }
}
