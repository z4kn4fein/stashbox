using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class FuncResolver : IEnumerableSupportedResolver
    {
        internal static readonly HashSet<Type> SupportedTypes = new HashSet<Type>
        {
            typeof(Func<>),
            typeof(Func<,>),
            typeof(Func<,,>),
            typeof(Func<,,,>),
            typeof(Func<,,,,>),
            typeof(Func<,,,,,>),
            typeof(Func<,,,,,,>),
            typeof(Func<,,,,,,,>)
        };

        public Expression GetExpression(IResolutionStrategy resolutionStrategy, TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var args = typeInfo.Type.GetGenericArguments();
            var wrappedType = args.LastElement();
            var funcArgumentInfo = typeInfo.CloneForType(wrappedType);

            var parameters = args.SelectButLast(a => a.AsParameter());
            var expression = resolutionStrategy.BuildExpressionForType(resolutionContext
                .BeginContextWithFunctionParameters(parameters), funcArgumentInfo);

            return expression?.AsLambda(typeInfo.Type, parameters);
        }

        public IEnumerable<Expression> GetExpressionsForEnumerableRequest(IResolutionStrategy resolutionStrategy, TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var type = typeInfo.Type;
            var args = type.GetGenericArguments();
            var wrappedType = args.LastElement();
            var funcArgumentInfo = typeInfo.CloneForType(wrappedType);

            var parameters = args.SelectButLast(a => a.AsParameter());
            return resolutionStrategy.BuildExpressionsForEnumerableRequest(resolutionContext
                    .BeginContextWithFunctionParameters(parameters), funcArgumentInfo)?
                .Select(e => e.AsLambda(type, parameters));
        }

        public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            typeInfo.Type.IsFuncType();
    }
}
