using Stashbox.Entity;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class EnumerableResolver : IResolver
    {
        public Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var enumerableType = new TypeInformation { Type = typeInfo.Type.GetEnumerableType() };
            var expressions = containerContext.ResolutionStrategy.BuildResolutionExpressions(containerContext, resolutionContext, enumerableType);

            return expressions == null ? enumerableType.Type.InitNewArray() :
                    enumerableType.Type.InitNewArray(expressions);
        }

        public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            typeInfo.Type.GetEnumerableType() != null;
    }
}
