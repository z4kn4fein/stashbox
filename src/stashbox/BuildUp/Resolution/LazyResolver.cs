using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Stashbox.Infrastructure.Registration;
using Stashbox.Infrastructure.Resolution;
using Stashbox.Utils;

namespace Stashbox.BuildUp.Resolution
{
    internal class LazyResolver : Resolver
    {
        private readonly TypeInformation lazyArgumentInfo;
        private readonly ConstructorInfo lazyConstructor;
        private readonly DelegateCache delegateCache;

        public LazyResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            this.lazyArgumentInfo = new TypeInformation
            {
                Type = typeInfo.Type.GenericTypeArguments[0],
                CustomAttributes = typeInfo.CustomAttributes,
                ParentType = typeInfo.ParentType,
                DependencyName = typeInfo.DependencyName
            };

            this.delegateCache = new DelegateCache(this.lazyArgumentInfo);
            var ctorParamType = typeof(Func<>).MakeGenericType(this.lazyArgumentInfo.Type);
            this.lazyConstructor = base.TypeInfo.Type.GetConstructor(ctorParamType);
        }

        public override Type WrappedType => this.lazyArgumentInfo.Type;

        public override bool CanUseForEnumerableArgumentResolution => true;

        public override Expression GetExpression(ResolutionInfo resolutionInfo)
        {
            var registration = this.BuilderContext.RegistrationRepository.GetRegistrationOrDefault(this.lazyArgumentInfo, true);
            if (registration != null)
                return !base.BuilderContext.Container.ContainerConfiguration.CircularDependenciesWithLazyEnabled ? 
                            Expression.New(this.lazyConstructor, Expression.Lambda(registration.GetExpression(resolutionInfo, this.lazyArgumentInfo))) :
                                this.CreateLazyExpressionCall(registration, resolutionInfo);

            Resolver resolver;
            if (!this.BuilderContext.ResolverSelector.TryChooseResolver(this.BuilderContext, this.lazyArgumentInfo, out resolver))
                throw new ResolutionFailedException(base.TypeInfo.Type.FullName);

            return Expression.New(this.lazyConstructor, Expression.Lambda(resolver.GetExpression(resolutionInfo)));
        }

        public override Expression[] GetEnumerableArgumentExpressions(ResolutionInfo resolutionInfo)
        {
            var registrations = this.BuilderContext.RegistrationRepository.GetRegistrationsOrDefault(this.lazyArgumentInfo);
            if (registrations != null)
            {
                var serviceRegistrations = base.BuilderContext.Container.ContainerConfiguration.EnumerableOrderRule(registrations).ToArray();
                var length = serviceRegistrations.Length;
                var expressions = new Expression[length];
                for (var i = 0; i < length; i++)
                    if(!base.BuilderContext.Container.ContainerConfiguration.CircularDependenciesWithLazyEnabled)
                        expressions[i] = Expression.New(this.lazyConstructor,
                            Expression.Lambda(serviceRegistrations[i].GetExpression(resolutionInfo, this.lazyArgumentInfo)));
                    else
                        expressions[i] = this.CreateLazyExpressionCall(serviceRegistrations[i], resolutionInfo);

                return expressions;
            }

            Resolver resolver;
            if (this.BuilderContext.ResolverSelector.TryChooseResolver(this.BuilderContext, this.lazyArgumentInfo, out resolver) &&
                resolver.CanUseForEnumerableArgumentResolution)
            {
                var exprs = resolver.GetEnumerableArgumentExpressions(resolutionInfo);
                var length = exprs.Length;
                var expressions = new Expression[length];
                for (var i = 0; i < length; i++)
                    expressions[i] = Expression.New(this.lazyConstructor, Expression.Lambda(exprs[i]));

                return expressions;
            }

            return null;
        }

        private Expression CreateLazyExpressionCall(IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo)
        {
            var callExpression = Expression.Call(Expression.Constant(this.delegateCache), "CreateLazyDelegate", null, 
                Expression.Constant(serviceRegistration), 
                Expression.Constant(resolutionInfo), 
                Expression.NewArrayInit(typeof(object), 
                resolutionInfo.ParameterExpressions?.OfType<Expression>() ?? new Expression[] { }));

            var convert = Expression.Convert(callExpression, this.lazyArgumentInfo.Type);
            return Expression.New(this.lazyConstructor, Expression.Lambda(convert));
        }
        
        private class DelegateCache
        {
            private readonly TypeInformation typeInformation;
            private readonly RandomAccessArray<Delegate> delegates;

            public DelegateCache(TypeInformation typeInformation)
            {
                this.typeInformation = typeInformation;
                this.delegates = new RandomAccessArray<Delegate>();
            }

            public object CreateLazyDelegate(IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, object[] arguments)
            {
                var cache = this.delegates.Load(serviceRegistration.RegistrationNumber);
                if (cache != null)
                    return cache.DynamicInvoke(arguments);

                var expr = serviceRegistration.GetExpression(resolutionInfo, this.typeInformation);
                return Expression.Lambda(expr, resolutionInfo.ParameterExpressions).Compile().DynamicInvoke(arguments);
            }
        }
    }
}
