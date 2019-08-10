using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Resolution
{
    internal interface IResolverSupportedResolutionStrategy : IResolutionStrategy
    {
        bool CanResolveType(IContainerContext containerContext, TypeInformation typeInfo,
            ResolutionContext resolutionContext);

        Expression BuildResolutionExpressionUsingResolvers(IContainerContext containerContext, TypeInformation typeInfo,
            ResolutionContext resolutionContext, bool forceSkipUnknownTypeCheck = false);

        void RegisterResolver(IResolver resolver);
    }
}
