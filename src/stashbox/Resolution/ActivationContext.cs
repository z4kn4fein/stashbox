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

        public object Activate(Type type, string name = null)
        {
            var cachedFactory = this.containerContext.DelegateRepository.GetDelegateCacheOrDefault(type, name);
            return cachedFactory != null ? cachedFactory() : this.ActivateType(ResolutionInfo.New(), type, name);
        }

        public Delegate ActivateFactory(Type type, Type[] parameterTypes, string name = null)
        {
            var cachedFactory = this.containerContext.DelegateRepository.GetFactoryDelegateCacheOrDefault(type, parameterTypes, name);
            return cachedFactory ?? ActivateFactoryDelegate(type, parameterTypes, name);
        }

        public object Activate(ResolutionInfo resolutionInfo, TypeInformation typeInfo) =>
            this.ActivateType(resolutionInfo, typeInfo);

        private object ActivateType(ResolutionInfo resolutionInfo, Type type, string name)
        {
            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(type, name);
            if (registration != null)
            {
                var ragistrationFactory = registration.GetExpression(resolutionInfo, type).CompileDelegate();
                this.containerContext.DelegateRepository.AddServiceDelegate(type, ragistrationFactory, name);
                return ragistrationFactory();
            }

            var expr = this.resolverSelector.GetResolverExpression(containerContext, new TypeInformation { Type = type, DependencyName = name }, resolutionInfo);
            if (expr == null)
                throw new ResolutionFailedException(type.FullName);

            var factory = expr.CompileDelegate();
            this.containerContext.DelegateRepository.AddServiceDelegate(type, factory, name);
            return factory();
        }

        private object ActivateType(ResolutionInfo resolutionInfo, TypeInformation typeInformation)
        {
            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInformation);
            if (registration != null)
                return registration.GetExpression(resolutionInfo, typeInformation.Type).CompileDelegate()();

            var expr = this.resolverSelector.GetResolverExpression(containerContext, typeInformation, resolutionInfo);
            if (expr == null)
                throw new ResolutionFailedException(typeInformation.Type.FullName);

            return expr.CompileDelegate()();
        }

        private Delegate ActivateFactoryDelegate(Type type, Type[] parameterTypes, string name = null)
        {
            var resolutionInfo = new ResolutionInfo
            {
                ParameterExpressions = parameterTypes.Length == 0 ? null : parameterTypes.Select(Expression.Parameter).ToArray()
            };
            var typeInfo = new TypeInformation { Type = type, DependencyName = name };
            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInfo);
            var initExpression = registration == null ?
                this.resolverSelector.GetResolverExpression(containerContext, typeInfo, resolutionInfo) :
                registration.GetExpression(resolutionInfo, type);

            if (initExpression == null)
                throw new ResolutionFailedException(typeInfo.Type.FullName);

            var factory = Expression.Lambda(initExpression, resolutionInfo.ParameterExpressions).CompileDelegate();
            this.containerContext.DelegateRepository.AddFactoryDelegate(type, parameterTypes, factory, name);
            return factory;
        }
    }
}