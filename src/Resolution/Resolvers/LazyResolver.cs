using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class LazyResolver : IEnumerableSupportedResolver, IWrapper
    {
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
            IsLazy(typeInfo.Type);

        public bool TryUnWrap(TypeInformation typeInfo, out IEnumerable<Type> unWrappedTypes)
        {
            if (!IsLazy(typeInfo.Type))
            {
                unWrappedTypes = null;
                return false;
            }

            unWrappedTypes = new[] { typeInfo.Type.GetGenericArguments()[0] };
            return true;
        }

        private Expression GetLazyFuncExpression(TypeInformation argumentType,
            ResolutionContext resolutionContext, IResolutionStrategy resolutionStrategy) =>
            resolutionContext.CurrentContainerContext.ContainerConfiguration.CircularDependenciesWithLazyEnabled
                ? resolutionContext.CurrentScopeParameter
                    .CallMethod(Constants.ResolveMethod, argumentType.Type.AsConstant(),
                        argumentType.DependencyName.AsConstant(), false.AsConstant(),
                        Constants.ObjectType.InitNewArray(resolutionContext.ParameterExpressions
                            .SelectMany(x => x.Select(i => i.I2.ConvertTo(Constants.ObjectType)))))
                    .ConvertTo(argumentType.Type)
                : resolutionStrategy.BuildExpressionForType(resolutionContext, argumentType);

        private static bool IsLazy(Type type) => type.IsClosedGenericType() && type.GetGenericTypeDefinition() == typeof(Lazy<>);
    }
}
