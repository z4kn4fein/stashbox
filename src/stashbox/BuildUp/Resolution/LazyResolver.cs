using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Infrastructure.Resolution;
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

        public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
        {
            var lazyArgumentInfo = typeInfo.Clone(typeInfo.Type.GetGenericArguments()[0]);

            var ctorParamType = Constants.FuncType.MakeGenericType(lazyArgumentInfo.Type);
            var lazyConstructor = typeInfo.Type.GetConstructor(ctorParamType);

            var registration = containerContext.RegistrationRepository.GetRegistrationOrDefault(lazyArgumentInfo, resolutionInfo.CurrentScopeName);
            if (registration != null)
                return !containerContext.ContainerConfigurator.ContainerConfiguration.CircularDependenciesWithLazyEnabled ?
                           Expression.New(lazyConstructor, Expression.Lambda(registration.GetExpression(containerContext, resolutionInfo, lazyArgumentInfo.Type))) :
                            CreateLazyExpressionCall(containerContext, registration, lazyArgumentInfo.Type, lazyConstructor, resolutionInfo);

            var expression = this.resolverSelector.GetResolverExpression(containerContext, lazyArgumentInfo, resolutionInfo);

            return expression == null ? null : Expression.New(lazyConstructor, Expression.Lambda(expression));
        }

        public override Expression[] GetExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
        {
            var lazyArgumentInfo = typeInfo.Clone(typeInfo.Type.GetGenericArguments()[0]);

            var ctorParamType = Constants.FuncType.MakeGenericType(lazyArgumentInfo.Type);
            var lazyConstructor = typeInfo.Type.GetConstructor(ctorParamType);

            var registrations = containerContext.RegistrationRepository.GetRegistrationsOrDefault(lazyArgumentInfo.Type, resolutionInfo.CurrentScopeName)?.CastToArray();
            if (registrations != null)
            {
                var regLength = registrations.Length;
                var regExpressions = new Expression[regLength];
                for (var i = 0; i < regLength; i++)
                    if (!containerContext.ContainerConfigurator.ContainerConfiguration.CircularDependenciesWithLazyEnabled)
                        regExpressions[i] = Expression.New(lazyConstructor,
                            Expression.Lambda(registrations[i].Value.GetExpression(containerContext, resolutionInfo, lazyArgumentInfo.Type)));
                    else
                        regExpressions[i] = CreateLazyExpressionCall(containerContext, registrations[i].Value, lazyArgumentInfo.Type, lazyConstructor, resolutionInfo);

                return regExpressions;
            }

            var exprs = this.resolverSelector.GetResolverExpressions(containerContext, lazyArgumentInfo, resolutionInfo);
            if (exprs == null)
                return null;

            var length = exprs.Length;
            var expressions = new Expression[length];
            for (var i = 0; i < length; i++)
                expressions[i] = Expression.New(lazyConstructor, Expression.Lambda(exprs[i]));

            return expressions;
        }

        private static Expression CreateLazyExpressionCall(IContainerContext containerContext, IServiceRegistration serviceRegistration, Type type, ConstructorInfo constructor, ResolutionInfo resolutionInfo)
        {
            var callExpression = Expression.Call(DelegateCacheMethod,
                Expression.Constant(containerContext),
                Expression.Constant(serviceRegistration),
                Expression.Constant(resolutionInfo),
                Expression.Constant(type),
                Expression.NewArrayInit(typeof(object),
                resolutionInfo.ParameterExpressions?.OfType<Expression>() ?? new Expression[] { }));

            var convert = Expression.Convert(callExpression, type);
            return Expression.New(constructor, Expression.Lambda(convert));
        }

        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo) =>
            typeInfo.Type.IsClosedGenericType() && typeInfo.Type.GetGenericTypeDefinition() == typeof(Lazy<>);

        private static readonly MethodInfo DelegateCacheMethod = typeof(LazyResolver).GetSingleMethod(nameof(CreateLazyDelegate), true);

        private static object CreateLazyDelegate(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type type, object[] arguments)
        {
            var expr = serviceRegistration.GetExpression(containerContext, resolutionInfo, type);
            return Expression.Lambda(expr, resolutionInfo.ParameterExpressions)
                .CompileDynamicDelegate(Constants.ScopeExpression)(resolutionInfo.ResolutionScope).DynamicInvoke(arguments);
        }
    }
}
