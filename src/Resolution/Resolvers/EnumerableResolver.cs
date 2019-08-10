using Stashbox.Entity;
using System;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class EnumerableResolver : IResolver
    {
        public Expression GetExpression(IContainerContext containerContext,
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var enumerableType = new TypeInformation { Type = typeInfo.Type.GetEnumerableType() };
            var expressions = resolutionStrategy.BuildAllResolutionExpressions(containerContext, resolutionContext, enumerableType);

            return expressions == null ? enumerableType.Type.InitNewArray() :
                    enumerableType.Type.InitNewArray(expressions);
        }

        public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            typeInfo.Type.GetEnumerableType() != null;
    }
}
