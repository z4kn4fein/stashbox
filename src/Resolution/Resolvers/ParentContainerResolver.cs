using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class ParentContainerResolver : IEnumerableSupportedResolver
    {
        public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            resolutionContext.CurrentContainerContext.ParentContext != null;

        public Expression GetExpression(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext) =>
            resolutionStrategy.BuildExpressionForType(resolutionContext.BeginCrossContainerContext(resolutionContext
                .CurrentContainerContext.ParentContext), typeInfo);

        public IEnumerable<Expression> GetExpressionsForEnumerableRequest(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext) =>
            resolutionStrategy.BuildExpressionsForEnumerableRequest(resolutionContext.BeginCrossContainerContext(resolutionContext
                .CurrentContainerContext.ParentContext), typeInfo);
    }
}
