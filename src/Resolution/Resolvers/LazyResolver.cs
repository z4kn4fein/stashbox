using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Resolution.Resolvers
{
    internal class LazyResolver : IEnumerableSupportedResolver
    {
        private static readonly MethodInfo ResolveMethodInfo =
            Constants.ResolverType.GetMethodByArguments(nameof(IDependencyResolver.Resolve), typeof(Type),
                Constants.ObjectType, typeof(bool), typeof(object[]));

        public Expression GetExpression(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var lazyArgumentInfo = typeInfo.CloneForType(typeInfo.Type.GetGenericArguments()[0]);

            var ctorParamType = Constants.FuncType.MakeGenericType(lazyArgumentInfo.Type);
            var lazyConstructor = typeInfo.Type.GetConstructor(ctorParamType);
            var expression = this.GetLazyFuncExpression(lazyArgumentInfo, resolutionContext, resolutionStrategy);

            return expression == null ? null : lazyConstructor.MakeNew(expression.AsLambda());
        }

        public IEnumerable<Expression> GetExpressionsForEnumerableRequest(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var lazyArgumentInfo = typeInfo.CloneForType(typeInfo.Type.GetGenericArguments()[0]);

            var ctorParamType = Constants.FuncType.MakeGenericType(lazyArgumentInfo.Type);
            var lazyConstructor = typeInfo.Type.GetConstructor(ctorParamType);

            return resolutionStrategy.BuildExpressionsForEnumerableRequest(resolutionContext, lazyArgumentInfo)
                ?.Select(e => lazyConstructor.MakeNew(e.AsLambda()));
        }

        public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            typeInfo.Type.IsClosedGenericType() &&
            typeInfo.Type.GetGenericTypeDefinition() == typeof(Lazy<>);

        private Expression GetLazyFuncExpression(TypeInformation argumentType,
            ResolutionContext resolutionContext, IResolutionStrategy resolutionStrategy) =>
            resolutionContext.CurrentContainerContext.ContainerConfiguration.CircularDependenciesWithLazyEnabled
                ? resolutionContext.CurrentScopeParameter.ConvertTo(Constants.ResolverType)
                    .CallMethod(ResolveMethodInfo, argumentType.Type.AsConstant(),
                        argumentType.DependencyName.AsConstant(), false.AsConstant(),
                        Constants.ObjectType.InitNewArray(resolutionContext.ParameterExpressions
                            .SelectMany(x => x.Select(i => i.Value.ConvertTo(Constants.ObjectType)))))
                    .ConvertTo(argumentType.Type)
                : resolutionStrategy.BuildExpressionForType(resolutionContext, argumentType);
    }
}
