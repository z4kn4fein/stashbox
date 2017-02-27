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
            return cachedFactory != null ? cachedFactory() : this.ActivateType(type, name);
        }

        public Delegate ActivateFactory(Type type, Type[] parameterTypes, string name = null)
        {
            var cachedFactory = this.containerContext.DelegateRepository.GetFactoryDelegateCacheOrDefault(type, parameterTypes, name);
            return cachedFactory ?? ActivateFactoryDelegate(type, parameterTypes, name);
        }

        public object Activate(ResolutionInfo resolutionInfo, TypeInformation typeInfo) =>
            this.ActivateType(resolutionInfo, typeInfo);

        private object ActivateType(Type type, string name = null)
        {
            var typeInfo = new TypeInformation { Type = type, DependencyName = name };
            return this.ActivateType(ResolutionInfo.New(), typeInfo);
        }

        private object ActivateType(ResolutionInfo resolutionInfo, TypeInformation typeInfo)
        {
            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInfo);
            if (registration != null)
            {
                var ragistrationFactory = registration.GetExpression(resolutionInfo, typeInfo).CompileDelegate();
                this.containerContext.DelegateRepository.AddServiceDelegate(typeInfo.Type, ragistrationFactory, typeInfo.DependencyName);
                return ragistrationFactory();
            }

            var expr = this.resolverSelector.GetResolverExpression(containerContext, typeInfo, resolutionInfo);
            if (expr == null)
                throw new ResolutionFailedException(typeInfo.Type.FullName);

            var factory = expr.CompileDelegate();
            this.containerContext.DelegateRepository.AddServiceDelegate(typeInfo.Type, factory, typeInfo.DependencyName);
            return factory();
        }

        private Delegate ActivateFactoryDelegate(Type type, Type[] parameterTypes, string name = null)
        {
            var resolutionInfo = new ResolutionInfo
            {
                ParameterExpressions = parameterTypes.Length == 0 ? null : parameterTypes.Select(Expression.Parameter).ToArray()
            };
            var typeInfo = new TypeInformation { Type = type, DependencyName = name };
            Expression initExpression;
            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInfo);
            if (registration == null)
                initExpression = this.resolverSelector.GetResolverExpression(containerContext, typeInfo, resolutionInfo);
            else
                initExpression = registration.GetExpression(resolutionInfo, typeInfo);

            if (initExpression == null)
                throw new ResolutionFailedException(typeInfo.Type.FullName);

            var factory = Expression.Lambda(initExpression, resolutionInfo.ParameterExpressions).Compile();
            this.containerContext.DelegateRepository.AddFactoryDelegate(type, parameterTypes, factory, name);
            return factory;
        }
    }
}
