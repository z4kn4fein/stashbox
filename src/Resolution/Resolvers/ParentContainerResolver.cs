using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class ParentContainerResolver : IMultiServiceResolver
    {
        public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            containerContext.Container.ParentContainer != null &&
            containerContext.Container.ParentContainer.CanResolve(typeInfo.Type, typeInfo.DependencyName);

        public Expression GetExpression(IContainerContext containerContext,
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var resolution = resolutionContext.RequestInitiatorContainerContext == null
                ? resolutionContext.BeginCrossContainerContext(containerContext, containerContext.Container.ParentContainer.ContainerContext)
                : resolutionContext.BeginCrossContainerContext(resolutionContext.RequestInitiatorContainerContext, containerContext.Container.ParentContainer.ContainerContext);

            var result = resolutionStrategy
                .BuildResolutionExpression(containerContext.Container.ParentContainer.ContainerContext, resolution, typeInfo);

            return result;
        }

        public Expression[] GetAllExpressions(IContainerContext containerContext,
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var resolution = resolutionContext.RequestInitiatorContainerContext == null
                ? resolutionContext.BeginCrossContainerContext(containerContext, containerContext.Container.ParentContainer.ContainerContext)
                : resolutionContext.BeginCrossContainerContext(resolutionContext.RequestInitiatorContainerContext, containerContext.Container.ParentContainer.ContainerContext);

            var result = resolutionStrategy
                .BuildAllResolutionExpressions(containerContext.Container.ParentContainer.ContainerContext, resolution, typeInfo);

            return result;
        }
    }
}
