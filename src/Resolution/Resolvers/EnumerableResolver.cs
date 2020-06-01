using System;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class EnumerableResolver : IResolver
    {
        public Expression GetExpression(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var enumerableType = typeInfo.CloneForType(typeInfo.Type.GetEnumerableType());
            var expressions = resolutionStrategy.BuildExpressionsForEnumerableRequest(resolutionContext, enumerableType);

            return expressions == null ? enumerableType.Type.InitNewArray() :
                    enumerableType.Type.InitNewArray(expressions);
        }

        public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            typeInfo.Type.GetEnumerableType() != null;
    }
}
