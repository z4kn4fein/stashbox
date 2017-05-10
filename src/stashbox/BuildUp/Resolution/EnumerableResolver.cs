using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;
using Stashbox.Infrastructure.Resolution;

namespace Stashbox.BuildUp.Resolution
{
    internal class EnumerableResolver : Resolver
    {
        public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
        {
            var enumerableType = new TypeInformation { Type = typeInfo.Type.GetEnumerableType() };
            var expressions = containerContext.ResolutionStrategy.BuildResolutionExpressions(containerContext, resolutionInfo, enumerableType);

            return expressions == null ? Expression.NewArrayInit(enumerableType.Type) :
                    Expression.NewArrayInit(enumerableType.Type, expressions);
        }
        
        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo) =>
            typeInfo.Type.GetEnumerableType() != null;
    }
}
