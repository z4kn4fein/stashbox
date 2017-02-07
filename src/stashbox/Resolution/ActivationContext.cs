using System;
using System.Linq.Expressions;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;

namespace Stashbox.Resolution
{
    internal class ActivationContext : IActivationContext
    {
        private readonly IResolverSelector resolverSelector;
        private readonly IContainerContext containerContext;

        public ActivationContext(IResolverSelector resolverSelector, IContainerContext containerContext)
        {
            this.resolverSelector = resolverSelector;
            this.containerContext = containerContext;
        }

        public object Activate(ResolutionInfo resolutionInfo, TypeInformation typeInfo)
        {
            var cachedFactory = this.containerContext.DelegateRepository.GetDelegateCacheOrDefault(typeInfo) ??
                this.containerContext.DelegateRepository.GetWrapperDelegateCacheOrDefault(typeInfo);
            if (cachedFactory != null)
                return cachedFactory();

            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInfo);
            if (registration != null)
            {
                var factory = this.CompileExpression(registration.GetExpression(resolutionInfo, typeInfo));
                this.containerContext.DelegateRepository.AddServiceDelegate(typeInfo, factory);
                return factory();
            }

            Resolver resolver;
            if (this.resolverSelector.TryChooseResolver(this.containerContext, typeInfo, out resolver))
            {
                var factory = this.CompileExpression(resolver.GetExpression(resolutionInfo));
                this.containerContext.DelegateRepository.AddWrapperDelegate(new WrappedDelegateInformation
                {
                    DependencyName = typeInfo.DependencyName,
                    WrappedType = resolver.WrappedType,
                    DelegateReturnType = typeInfo.Type
                }, factory);
                return factory();
            }

            throw new ResolutionFailedException(typeInfo.Type.FullName);
        }

        public Delegate ActivateFactory(ResolutionInfo resolutionInfo, TypeInformation typeInfo, Type parameterType)
        {
            var cachedFactory = this.containerContext.DelegateRepository.GetFactoryDelegateCacheOrDefault(typeInfo, parameterType);
            if (cachedFactory != null)
                return cachedFactory;

            Expression initExpression;
            var registration = containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInfo);
            if (registration == null)
            {
                Resolver resolver;
                if (!this.resolverSelector.TryChooseResolver(this.containerContext, typeInfo, out resolver))
                    throw new ResolutionFailedException(typeInfo.Type.FullName);

                initExpression = resolver.GetExpression(resolutionInfo);
            }
            else
                initExpression = registration.GetExpression(resolutionInfo, typeInfo);
            
            var delegateType = typeof(Func<,>).MakeGenericType(parameterType, typeInfo.Type);
            var factory = Expression.Lambda(delegateType, initExpression, resolutionInfo.ParameterExpressions).Compile();
            this.containerContext.DelegateRepository.AddFactoryDelegate(typeInfo, parameterType, factory);
            return factory;
        }

        private Func<object> CompileExpression(Expression expression)
        {
            Func<object> factory;
            if (expression.NodeType == ExpressionType.Constant)
            {
                var instance = ((ConstantExpression)expression).Value;
                factory = () => instance;
            }
            else
                factory = Expression.Lambda<Func<object>>(expression).Compile();

            return factory;
        }
    }
}
