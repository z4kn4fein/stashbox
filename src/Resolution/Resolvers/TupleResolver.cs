using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class TupleResolver : IResolver, IWrapper
    {
        private static readonly HashSet<Type> SupportedTypes = new()
        {
            typeof(Tuple<>),
            typeof(Tuple<,>),
            typeof(Tuple<,,>),
            typeof(Tuple<,,,>),
            typeof(Tuple<,,,,>),
            typeof(Tuple<,,,,,>),
            typeof(Tuple<,,,,,,>),
            typeof(Tuple<,,,,,,,>)
        };

        public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            IsTuple(typeInfo.Type);

        public Expression GetExpression(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var args = typeInfo.Type.GetGenericArguments();
            var tupleConstructor = typeInfo.Type.GetConstructor(args);
            var length = args.Length;
            var expressions = new Expression[length];
            for (var i = 0; i < length; i++)
            {
                var argumentInfo = typeInfo.CloneForType(args[i]);
                var expr = resolutionStrategy.BuildExpressionForType(resolutionContext, argumentInfo);

                if (expr != null)
                    expressions[i] = expr;
                else
                    return null;
            }

            return tupleConstructor.MakeNew(expressions);
        }

        public bool TryUnWrap(TypeInformation typeInfo, out IEnumerable<Type> unWrappedTypes)
        {
            if (!IsTuple(typeInfo.Type))
            {
                unWrappedTypes = null;
                return false;
            }

            unWrappedTypes = typeInfo.Type.GetGenericArguments();
            return true;
        }

        private static bool IsTuple(Type type) => type.IsClosedGenericType() && SupportedTypes.Contains(type.GetGenericTypeDefinition());
    }
}
