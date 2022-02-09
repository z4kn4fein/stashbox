using System.Collections.Generic;

namespace Stashbox.Resolution.Resolvers
{
    internal class ParentContainerResolver : IEnumerableSupportedResolver, ILookup
    {
        public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            resolutionContext.CurrentContainerContext.ParentContext != null;

        public ServiceContext GetExpression(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext) =>
            resolutionStrategy.BuildExpressionForType(resolutionContext.BeginCrossContainerContext(resolutionContext
                .CurrentContainerContext.ParentContext), typeInfo);

        public IEnumerable<ServiceContext> GetExpressionsForEnumerableRequest(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext) =>
            resolutionStrategy.BuildExpressionsForEnumerableRequest(resolutionContext.BeginCrossContainerContext(resolutionContext
                .CurrentContainerContext.ParentContext), typeInfo);

        public bool CanLookupService(TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            if (resolutionContext.CurrentContainerContext.ParentContext == null)
                return false;

            return resolutionContext.CurrentContainerContext.ResolutionStrategy.IsTypeResolvable(resolutionContext.BeginCrossContainerContext(resolutionContext
                .CurrentContainerContext.ParentContext), typeInfo);
        }
    }
}
