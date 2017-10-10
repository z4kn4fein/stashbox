using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;
using Stashbox.Infrastructure.Resolution;
using Stashbox.Resolution;

namespace Stashbox.BuildUp.Resolution
{
    internal class EnumerableResolver : Resolver
    {
        public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var enumerableType = new TypeInformation { Type = typeInfo.Type.GetEnumerableType() };
            var expressions = containerContext.ResolutionStrategy.BuildResolutionExpressions(containerContext, resolutionContext, enumerableType);

            return expressions == null ? Expression.NewArrayInit(enumerableType.Type) :
                    Expression.NewArrayInit(enumerableType.Type, expressions);
        }
        
        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            typeInfo.Type.GetEnumerableType() != null;
    }
}
