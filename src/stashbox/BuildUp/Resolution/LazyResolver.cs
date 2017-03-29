using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Stashbox.Infrastructure.Registration;
using Stashbox.Infrastructure.Resolution;

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

            var registration = containerContext.RegistrationRepository.GetRegistrationOrDefault(lazyArgumentInfo, true);
            if (registration != null)
                return !containerContext.ContainerConfigurator.ContainerConfiguration.CircularDependenciesWithLazyEnabled ?
                           Expression.New(lazyConstructor, Expression.Lambda(registration.GetExpression(resolutionInfo, lazyArgumentInfo.Type))) :
                            this.CreateLazyExpressionCall(registration, lazyArgumentInfo.Type, lazyConstructor, resolutionInfo);

            var expression = this.resolverSelector.GetResolverExpression(containerContext, lazyArgumentInfo, resolutionInfo);

            return expression == null ? null : Expression.New(lazyConstructor, Expression.Lambda(expression));
        }

        public override Expression[] GetExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
        {
            var lazyArgumentInfo = typeInfo.Clone(typeInfo.Type.GetGenericArguments()[0]);

            var ctorParamType = Constants.FuncType.MakeGenericType(lazyArgumentInfo.Type);
            var lazyConstructor = typeInfo.Type.GetConstructor(ctorParamType);

            var registrations = containerContext.RegistrationRepository.GetRegistrationsOrDefault(lazyArgumentInfo.Type);
            if (registrations != null)
            {
                var serviceRegistrations = containerContext.ContainerConfigurator.ContainerConfiguration.EnumerableOrderRule(registrations).ToArray();
                var regLenght = serviceRegistrations.Length;
                var regExpressions = new Expression[regLenght];
                for (var i = 0; i < regLenght; i++)
                    if (!containerContext.ContainerConfigurator.ContainerConfiguration.CircularDependenciesWithLazyEnabled)
                        regExpressions[i] = Expression.New(lazyConstructor,
                            Expression.Lambda(serviceRegistrations[i].GetExpression(resolutionInfo, lazyArgumentInfo.Type)));
                    else
                        regExpressions[i] = this.CreateLazyExpressionCall(serviceRegistrations[i], lazyArgumentInfo.Type, lazyConstructor, resolutionInfo);
                
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

        private Expression CreateLazyExpressionCall(IServiceRegistration serviceRegistration, Type type, ConstructorInfo constructor, ResolutionInfo resolutionInfo)
        {
            var delegateCache = new DelegateCache(type);
            var callExpression = Expression.Call(Expression.Constant(delegateCache), 
                DelegateCacheMethod,
                Expression.Constant(serviceRegistration),
                Expression.Constant(resolutionInfo),
                Expression.NewArrayInit(typeof(object),
                resolutionInfo.ParameterExpressions?.OfType<Expression>() ?? new Expression[] { }));

            var convert = Expression.Convert(callExpression, type);
            return Expression.New(constructor, Expression.Lambda(convert));
        }

        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo) =>
            typeInfo.Type.IsClosedGenericType() && typeInfo.Type.GetGenericTypeDefinition() == typeof(Lazy<>);

        private static readonly MethodInfo DelegateCacheMethod = typeof(DelegateCache).GetSingleMethod("CreateLazyDelegate");

        private class DelegateCache
        {
            private readonly Type type;
            private Func<IResolutionScope, Delegate> cache;

            public DelegateCache(Type type)
            {
                this.type = type;
            }

            public object CreateLazyDelegate(IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, object[] arguments)
            {
                if (this.cache != null)
                    return this.cache(resolutionInfo.ResolutionScope).DynamicInvoke(arguments);

                var expr = serviceRegistration.GetExpression(resolutionInfo, this.type);
                this.cache = Expression.Lambda(expr, resolutionInfo.ParameterExpressions).CompileDelegate(Constants.ScopeExpression);
                return this.cache(resolutionInfo.ResolutionScope).DynamicInvoke(arguments);
            }
        }
    }
}
