using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class EnumerableResolver : IResolver, IWrapper
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

        public bool TryUnWrap(TypeInformation typeInfo, out IEnumerable<Type> unWrappedTypes)
        {
            var enumerableType = typeInfo.Type.GetEnumerableType();
            if (enumerableType == null)
            {
                unWrappedTypes = null;
                return false;
            }

            unWrappedTypes = new []{ enumerableType };
            return true;
        }
    }
}
