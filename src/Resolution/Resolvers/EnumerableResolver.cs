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
            var newTypeInfo = typeInfo.CloneForType(typeInfo.Type.GetEnumerableType());
            var expressions = resolutionStrategy.BuildExpressionsForEnumerableRequest(resolutionContext, newTypeInfo);

            return expressions == null ? newTypeInfo.Type.InitNewArray() :
                newTypeInfo.Type.InitNewArray(expressions);
        }

        public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            typeInfo.Type.GetEnumerableType() != null;
    }
}
