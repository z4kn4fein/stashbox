using System.Collections.Generic;

namespace Stashbox.Resolution.Resolvers;

internal class ParentContainerResolver : IEnumerableSupportedResolver, ILookup
{
    public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
        resolutionContext.CurrentContainerContext.ParentContext != null && 
        (resolutionContext.ResolutionBehavior.Has(ResolutionBehavior.Parent) || 
         typeInfo.IsDependency && resolutionContext.ResolutionBehavior.Has(ResolutionBehavior.ParentDependency));

    public ServiceContext GetExpression(
        IResolutionStrategy resolutionStrategy,
        TypeInformation typeInfo,
        ResolutionContext resolutionContext) =>
        resolutionStrategy.BuildExpressionForType(resolutionContext.BeginParentContainerContext(resolutionContext
            .CurrentContainerContext.ParentContext!), typeInfo);

    public IEnumerable<ServiceContext> GetExpressionsForEnumerableRequest(
        IResolutionStrategy resolutionStrategy,
        TypeInformation typeInfo,
        ResolutionContext resolutionContext) =>
        resolutionStrategy.BuildExpressionsForEnumerableRequest(resolutionContext.BeginParentContainerContext(resolutionContext
            .CurrentContainerContext.ParentContext!), typeInfo);

    public bool CanLookupService(TypeInformation typeInfo, ResolutionContext resolutionContext)
    {
        if (resolutionContext.CurrentContainerContext.ParentContext == null || !resolutionContext.ResolutionBehavior.Has(ResolutionBehavior.Parent))
            return false;

        return resolutionContext.CurrentContainerContext.ResolutionStrategy.IsTypeResolvable(resolutionContext.BeginParentContainerContext(resolutionContext
            .CurrentContainerContext.ParentContext), typeInfo);
    }
}