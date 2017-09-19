using System;
using System.Linq.Expressions;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;
using System.Linq;

namespace Stashbox.Resolution
{
    internal class ActivationContext : IActivationContext
    {
        private readonly IContainerContext containerContext;
        private readonly IResolverSelector resolverSelector;

        public ActivationContext(IContainerContext containerContext, IResolverSelector resolverSelector)
        {
            this.containerContext = containerContext;
            this.resolverSelector = resolverSelector;
        }

        public object Activate(Type type, IResolutionScope resolutionScope, bool nullResultAllowed = false)
        {
            var cachedFactory = this.containerContext.DelegateRepository.GetDelegateCacheOrDefault(type);
            return cachedFactory != null ? cachedFactory(resolutionScope) : this.Activate(ResolutionInfo.New(resolutionScope, nullResultAllowed), type);
        }

        public object Activate(Type type, IResolutionScope resolutionScope, object name, bool nullResultAllowed = false)
        {
            var cachedFactory = this.containerContext.DelegateRepository.GetDelegateCacheOrDefault(name);
            return cachedFactory != null ? cachedFactory(resolutionScope) : this.Activate(ResolutionInfo.New(resolutionScope, nullResultAllowed), type, name);
        }

        public Delegate ActivateFactory(Type type, Type[] parameterTypes, IResolutionScope resolutionScope, object name = null, bool nullResultAllowed = false)
        {
            var cachedFactory = this.containerContext.DelegateRepository.GetFactoryDelegateCacheOrDefault(type, name);
            return cachedFactory != null ? cachedFactory(resolutionScope) : this.ActivateFactoryDelegate(type, parameterTypes, resolutionScope, name, nullResultAllowed);
        }

        private object Activate(ResolutionInfo resolutionInfo, Type type, object name = null)
        {
            if (type == Constants.ResolverType)
                return resolutionInfo.ResolutionScope;

            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(type, name);
            if (registration != null)
            {
                var ragistrationFactory = registration.GetExpression(resolutionInfo, type)?.CompileDelegate(Constants.ScopeExpression);
                if (ragistrationFactory == null)
                    if (resolutionInfo.NullResultAllowed)
                        return null;
                    else
                        throw new ResolutionFailedException(type);

                this.containerContext.DelegateRepository.AddServiceDelegate(type, ragistrationFactory, name);
                return ragistrationFactory(resolutionInfo.ResolutionScope);
            }

            var expr = this.resolverSelector.GetResolverExpression(this.containerContext, new TypeInformation { Type = type, DependencyName = name }, resolutionInfo);
            if (expr == null)
                if (resolutionInfo.NullResultAllowed)
                    return null;
                else
                    throw new ResolutionFailedException(type);

            var factory = expr.CompileDelegate(Constants.ScopeExpression);
            this.containerContext.DelegateRepository.AddServiceDelegate(type, factory, name);
            return factory(resolutionInfo.ResolutionScope);
        }

        private Delegate ActivateFactoryDelegate(Type type, Type[] parameterTypes, IResolutionScope resolutionScope, object name, bool nullResultAllowed)
        {
            var resolutionInfo = new ResolutionInfo(resolutionScope, nullResultAllowed)
            {
                ParameterExpressions = parameterTypes.Length == 0 ? null : parameterTypes.Select(Expression.Parameter).ToArray()
            };

            var typeInfo = new TypeInformation { Type = type, DependencyName = name };
            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInfo);

            var initExpression = registration == null ?
                this.resolverSelector.GetResolverExpression(this.containerContext, typeInfo, resolutionInfo) :
                registration.GetExpression(resolutionInfo, type);

            if (initExpression == null)
                if (resolutionInfo.NullResultAllowed)
                    return null;
                else
                    throw new ResolutionFailedException(type);

            var expression = Expression.Lambda(initExpression, resolutionInfo.ParameterExpressions);

            var factory = expression.CompileDelegate(Constants.ScopeExpression);
            this.containerContext.DelegateRepository.AddFactoryDelegate(type, factory, name);
            return factory(resolutionScope);
        }
    }
}