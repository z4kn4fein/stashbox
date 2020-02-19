using Stashbox.Entity;
using Stashbox.Registration;
using Stashbox.Utils;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Resolution.Resolvers
{
    internal class LazyResolver : IMultiServiceResolver
    {
        public Expression GetExpression(IContainerContext containerContext,
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var lazyArgumentInfo = typeInfo.Clone(typeInfo.Type.GetGenericArguments()[0]);

            var ctorParamType = Constants.FuncType.MakeGenericType(lazyArgumentInfo.Type);
            var lazyConstructor = typeInfo.Type.GetConstructor(ctorParamType);

            var registration = containerContext.RegistrationRepository.GetRegistrationOrDefault(lazyArgumentInfo, resolutionContext);
            if (registration != null)
                return !containerContext.ContainerConfiguration.CircularDependenciesWithLazyEnabled ?
                           lazyConstructor.MakeNew(registration.GetExpression(containerContext, resolutionContext, lazyArgumentInfo.Type).AsLambda()) :
                            CreateLazyExpressionCall(containerContext, registration, lazyArgumentInfo.Type, lazyConstructor, resolutionContext);

            var expression = resolutionStrategy.BuildResolutionExpression(containerContext, resolutionContext, lazyArgumentInfo);

            return expression == null ? null : lazyConstructor.MakeNew(expression.AsLambda());
        }

        public Expression[] GetAllExpressions(IContainerContext containerContext,
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var lazyArgumentInfo = typeInfo.Clone(typeInfo.Type.GetGenericArguments()[0]);

            var ctorParamType = Constants.FuncType.MakeGenericType(lazyArgumentInfo.Type);
            var lazyConstructor = typeInfo.Type.GetConstructor(ctorParamType);

            var registrations = containerContext.RegistrationRepository.GetRegistrationsOrDefault(lazyArgumentInfo, resolutionContext)?.CastToArray();
            if (registrations != null)
            {
                var regLength = registrations.Length;
                var regExpressions = new Expression[regLength];
                for (var i = 0; i < regLength; i++)
                    if (!containerContext.ContainerConfiguration.CircularDependenciesWithLazyEnabled)
                        regExpressions[i] = lazyConstructor.MakeNew(registrations[i].Value.GetExpression(containerContext, resolutionContext, lazyArgumentInfo.Type).AsLambda());
                    else
                        regExpressions[i] = CreateLazyExpressionCall(containerContext, registrations[i].Value, lazyArgumentInfo.Type, lazyConstructor, resolutionContext);

                return regExpressions;
            }

            var exprs = resolutionStrategy.BuildAllResolutionExpressions(containerContext, resolutionContext, lazyArgumentInfo);
            if (exprs == null)
                return null;

            var length = exprs.Length;
            var expressions = new Expression[length];
            for (var i = 0; i < length; i++)
                expressions[i] = lazyConstructor.MakeNew(exprs[i].AsLambda());

            return expressions;
        }

        private static Expression CreateLazyExpressionCall(IContainerContext containerContext, IServiceRegistration serviceRegistration, Type type, ConstructorInfo constructor, ResolutionContext resolutionContext)
        {
            var arguments = resolutionContext.ParameterExpressions != null
                ? new Expression[resolutionContext.ParameterExpressions.Sum(x => x.Length)]
                : new Expression[0];

            if (resolutionContext.ParameterExpressions != null)
            {
                var index = 0;
                for (var i = 0; i < resolutionContext.ParameterExpressions.Length; i++)
                    for (var j = 0; j < resolutionContext.ParameterExpressions[i].Length; j++)
                        arguments[index++] = resolutionContext.ParameterExpressions[i][j].ConvertTo(typeof(object));
            }



            var callExpression = DelegateCacheMethod.CallStaticMethod(
                containerContext.AsConstant(),
                serviceRegistration.AsConstant(),
                resolutionContext.AsConstant(),
                type.AsConstant(),
                typeof(object).InitNewArray(arguments));

            return constructor.MakeNew(callExpression.ConvertTo(type).AsLambda());
        }

        public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            typeInfo.Type.IsClosedGenericType() &&
            typeInfo.Type.GetGenericTypeDefinition() == typeof(Lazy<>);

        private static readonly MethodInfo DelegateCacheMethod = typeof(LazyResolver).GetSingleMethod(nameof(CreateLazyDelegate), true);

        private static object CreateLazyDelegate(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type type, object[] arguments)
        {
            var expr = serviceRegistration.GetExpression(containerContext, resolutionContext, type);
            return expr.AsLambda(resolutionContext.ParameterExpressions.SelectMany(x => x))
                .CompileDynamicDelegate(resolutionContext, containerContext.ContainerConfiguration)(resolutionContext.ResolutionScope).DynamicInvoke(arguments);
        }
    }
}
