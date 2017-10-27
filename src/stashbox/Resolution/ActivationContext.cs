using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;
using System;
using System.Linq;
using System.Linq.Expressions;

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
            return cachedFactory != null ? cachedFactory(resolutionScope) : this.Activate(ResolutionContext.New(resolutionScope, nullResultAllowed), type);
        }

        public object Activate(Type type, IResolutionScope resolutionScope, object name, bool nullResultAllowed = false)
        {
            var cachedFactory = this.containerContext.DelegateRepository.GetDelegateCacheOrDefault(name);
            return cachedFactory != null ? cachedFactory(resolutionScope) : this.Activate(ResolutionContext.New(resolutionScope, nullResultAllowed), type, name);
        }

        public Delegate ActivateFactory(Type type, Type[] parameterTypes, IResolutionScope resolutionScope, object name = null, bool nullResultAllowed = false)
        {
            var cachedFactory = this.containerContext.DelegateRepository.GetFactoryDelegateCacheOrDefault(type, name);
            return cachedFactory != null ? cachedFactory(resolutionScope) : this.ActivateFactoryDelegate(type, parameterTypes, resolutionScope, name, nullResultAllowed);
        }

        private object Activate(ResolutionContext resolutionContext, Type type, object name = null)
        {
            if (type == Constants.ResolverType)
                return resolutionContext.ResolutionScope;

            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(type, resolutionContext, name);
            if (registration != null)
            {
                var ragistrationFactory = registration.GetExpression(this.containerContext, resolutionContext, type)?.CompileDelegate(resolutionContext);
                if (ragistrationFactory == null)
                    if (resolutionContext.NullResultAllowed)
                        return null;
                    else
                        throw new ResolutionFailedException(type);

                this.containerContext.DelegateRepository.AddServiceDelegate(type, ragistrationFactory, name);
                return ragistrationFactory(resolutionContext.ResolutionScope);
            }

            var expr = this.resolverSelector.GetResolverExpression(this.containerContext, new TypeInformation { Type = type, DependencyName = name }, resolutionContext);
            if (expr == null)
                if (resolutionContext.NullResultAllowed)
                    return null;
                else
                    throw new ResolutionFailedException(type);

            var factory = expr.CompileDelegate(resolutionContext);
            this.containerContext.DelegateRepository.AddServiceDelegate(type, factory, name);
            return factory(resolutionContext.ResolutionScope);
        }

        private Delegate ActivateFactoryDelegate(Type type, Type[] parameterTypes, IResolutionScope resolutionScope, object name, bool nullResultAllowed)
        {
            var resolutionContext = ResolutionContext.New(resolutionScope, nullResultAllowed);
            resolutionContext.AddParameterExpressions(parameterTypes.Select(Expression.Parameter).ToArray());

            var typeInfo = new TypeInformation { Type = type, DependencyName = name };
            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInfo, resolutionContext);

            var initExpression = registration == null ?
                this.resolverSelector.GetResolverExpression(this.containerContext, typeInfo, resolutionContext) :
                registration.GetExpression(this.containerContext, resolutionContext, type);

            if (initExpression == null)
                if (resolutionContext.NullResultAllowed)
                    return null;
                else
                    throw new ResolutionFailedException(type);

            var expression = initExpression.AsLambda(resolutionContext.ParameterExpressions);

            var factory = expression.CompileDynamicDelegate(resolutionContext);
            this.containerContext.DelegateRepository.AddFactoryDelegate(type, factory, name);
            return factory(resolutionScope);
        }
    }
}