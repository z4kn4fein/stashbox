using Stashbox.Entity;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Resolution
{
    internal class LazyResolver : Resolver
    {
        private readonly IResolverSelector resolverSelector;

        public override bool SupportsMany => true;

        public LazyResolver(IResolverSelector resolverSelector)
        {
            this.resolverSelector = resolverSelector;
        }

        public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var lazyArgumentInfo = typeInfo.Clone(typeInfo.Type.GetGenericArguments()[0]);

            var ctorParamType = Constants.FuncType.MakeGenericType(lazyArgumentInfo.Type);
            var lazyConstructor = typeInfo.Type.GetConstructor(ctorParamType);

            var registration = containerContext.RegistrationRepository.GetRegistrationOrDefault(lazyArgumentInfo, resolutionContext);
            if (registration != null)
                return !containerContext.ContainerConfigurator.ContainerConfiguration.CircularDependenciesWithLazyEnabled ?
                           lazyConstructor.MakeNew(registration.GetExpression(containerContext, resolutionContext, lazyArgumentInfo.Type).AsLambda()) :
                            CreateLazyExpressionCall(containerContext, registration, lazyArgumentInfo.Type, lazyConstructor, resolutionContext);

            var expression = this.resolverSelector.GetResolverExpression(containerContext, lazyArgumentInfo, resolutionContext);

            return expression == null ? null : lazyConstructor.MakeNew(expression.AsLambda());
        }

        public override Expression[] GetExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var lazyArgumentInfo = typeInfo.Clone(typeInfo.Type.GetGenericArguments()[0]);

            var ctorParamType = Constants.FuncType.MakeGenericType(lazyArgumentInfo.Type);
            var lazyConstructor = typeInfo.Type.GetConstructor(ctorParamType);

            var registrations = containerContext.RegistrationRepository.GetRegistrationsOrDefault(lazyArgumentInfo.Type, resolutionContext)?.CastToArray();
            if (registrations != null)
            {
                var regLength = registrations.Length;
                var regExpressions = new Expression[regLength];
                for (var i = 0; i < regLength; i++)
                    if (!containerContext.ContainerConfigurator.ContainerConfiguration.CircularDependenciesWithLazyEnabled)
                        regExpressions[i] = lazyConstructor.MakeNew(registrations[i].Value.GetExpression(containerContext, resolutionContext, lazyArgumentInfo.Type).AsLambda());
                    else
                        regExpressions[i] = CreateLazyExpressionCall(containerContext, registrations[i].Value, lazyArgumentInfo.Type, lazyConstructor, resolutionContext);

                return regExpressions;
            }

            var exprs = this.resolverSelector.GetResolverExpressions(containerContext, lazyArgumentInfo, resolutionContext);
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



            var callExpression = DelegateCacheMethod.InvokeMethod(
                containerContext.AsConstant(),
                serviceRegistration.AsConstant(),
                resolutionContext.AsConstant(),
                type.AsConstant(),
                typeof(object).InitNewArray(arguments));

            return constructor.MakeNew(callExpression.ConvertTo(type).AsLambda());
        }

        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            typeInfo.Type.IsClosedGenericType() && typeInfo.Type.GetGenericTypeDefinition() == typeof(Lazy<>);

        private static readonly MethodInfo DelegateCacheMethod = typeof(LazyResolver).GetSingleMethod(nameof(CreateLazyDelegate), true);

        private static object CreateLazyDelegate(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type type, object[] arguments)
        {
            var expr = serviceRegistration.GetExpression(containerContext, resolutionContext, type);
            return expr.AsLambda(resolutionContext.ParameterExpressions.SelectMany(x => x))
                .CompileDynamicDelegate(resolutionContext)(resolutionContext.ResolutionScope).DynamicInvoke(arguments);
        }
    }
}
