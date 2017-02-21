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
            var lazyArgumentInfo = new TypeInformation
            {
                Type = typeInfo.Type.GenericTypeArguments[0],
                CustomAttributes = typeInfo.CustomAttributes,
                ParentType = typeInfo.ParentType,
                DependencyName = typeInfo.DependencyName
            };

            var delegateCache = new DelegateCache(lazyArgumentInfo);
            var ctorParamType = typeof(Func<>).MakeGenericType(lazyArgumentInfo.Type);
            var lazyConstructor = typeInfo.Type.GetConstructor(ctorParamType);

            var registration = containerContext.RegistrationRepository.GetRegistrationOrDefault(lazyArgumentInfo, true);
            if (registration != null)
                return !containerContext.ContainerConfigurator.ContainerConfiguration.CircularDependenciesWithLazyEnabled ?
                           Expression.New(lazyConstructor, Expression.Lambda(registration.GetExpression(resolutionInfo, lazyArgumentInfo))) :
                            this.CreateLazyExpressionCall(registration, lazyArgumentInfo, lazyConstructor, resolutionInfo);

            var expression = this.resolverSelector.GetResolverExpression(containerContext, lazyArgumentInfo, resolutionInfo);
            return Expression.New(lazyConstructor, Expression.Lambda(expression));
        }

        public override Expression[] GetExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
        {
            var lazyArgumentInfo = new TypeInformation
            {
                Type = typeInfo.Type.GenericTypeArguments[0],
                CustomAttributes = typeInfo.CustomAttributes,
                ParentType = typeInfo.ParentType,
                DependencyName = typeInfo.DependencyName
            };

            var delegateCache = new DelegateCache(lazyArgumentInfo);
            var ctorParamType = typeof(Func<>).MakeGenericType(lazyArgumentInfo.Type);
            var lazyConstructor = typeInfo.Type.GetConstructor(ctorParamType);

            var registrations = containerContext.RegistrationRepository.GetRegistrationsOrDefault(lazyArgumentInfo);
            if (registrations != null)
            {
                var serviceRegistrations = containerContext.ContainerConfigurator.ContainerConfiguration.EnumerableOrderRule(registrations).ToArray();
                var regLenght = serviceRegistrations.Length;
                var regExpressions = new Expression[regLenght];
                for (var i = 0; i < regLenght; i++)
                    if (!containerContext.ContainerConfigurator.ContainerConfiguration.CircularDependenciesWithLazyEnabled)
                        regExpressions[i] = Expression.New(lazyConstructor,
                            Expression.Lambda(serviceRegistrations[i].GetExpression(resolutionInfo, lazyArgumentInfo)));
                    else
                        regExpressions[i] = this.CreateLazyExpressionCall(serviceRegistrations[i], lazyArgumentInfo, lazyConstructor, resolutionInfo);
                
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

        private Expression CreateLazyExpressionCall(IServiceRegistration serviceRegistration, TypeInformation typeInfo, ConstructorInfo constructor, ResolutionInfo resolutionInfo)
        {
            var delegateCache = new DelegateCache(typeInfo);
            var callExpression = Expression.Call(Expression.Constant(delegateCache), "CreateLazyDelegate", null,
                Expression.Constant(serviceRegistration),
                Expression.Constant(resolutionInfo),
                Expression.NewArrayInit(typeof(object),
                resolutionInfo.ParameterExpressions?.OfType<Expression>() ?? new Expression[] { }));

            var convert = Expression.Convert(callExpression, typeInfo.Type);
            return Expression.New(constructor, Expression.Lambda(convert));
        }

        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo) =>
            typeInfo.Type.IsConstructedGenericType && typeInfo.Type.GetGenericTypeDefinition() == typeof(Lazy<>);

        private class DelegateCache
        {
            private readonly TypeInformation typeInformation;
            private Delegate cache;

            public DelegateCache(TypeInformation typeInformation)
            {
                this.typeInformation = typeInformation;
            }

            public object CreateLazyDelegate(IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, object[] arguments)
            {
                if (this.cache != null)
                    return this.cache.DynamicInvoke(arguments);

                var expr = serviceRegistration.GetExpression(resolutionInfo, this.typeInformation);
                this.cache = Expression.Lambda(expr, resolutionInfo.ParameterExpressions).Compile();
                return this.cache.DynamicInvoke(arguments);
            }
        }
    }
}
