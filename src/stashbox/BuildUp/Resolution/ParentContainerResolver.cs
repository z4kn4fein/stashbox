using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;
using Stashbox.Resolution;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class ParentContainerResolver : Resolver
    {
        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            containerContext.Container.ParentContainer != null && containerContext.Container.ParentContainer.CanResolve(typeInfo.Type, typeInfo.DependencyName);

        public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            containerContext.Container.ParentContainer.ContainerContext.ResolutionStrategy
                .BuildResolutionExpression(containerContext.Container.ParentContainer.ContainerContext,
                resolutionContext.ChildContext == null
                    ? resolutionContext.CreateNew(containerContext)
                    : resolutionContext,
                typeInfo, null);

        public override Expression[] GetExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            containerContext.Container.ParentContainer.ContainerContext.ResolutionStrategy
                .BuildResolutionExpressions(containerContext.Container.ParentContainer.ContainerContext,
                resolutionContext.ChildContext == null
                    ? resolutionContext.CreateNew(containerContext)
                    : resolutionContext,
                typeInfo);
    }
}
