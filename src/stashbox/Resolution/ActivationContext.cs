using System;
using System.Linq.Expressions;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;

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
            if (resolutionInfo.OverrideManager == null)
            {
                var factory = this.containerContext.DelegateRepository.GetOrDefault(typeInfo.Type);
                if (factory != null)
                    return factory();
            }

            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInfo);
            if (registration != null)
                return registration.GetFactory(resolutionInfo, typeInfo)();

            Resolver resolver;
            if (this.resolverSelector.TryChooseResolver(this.containerContext, typeInfo, out resolver))
                return this.CompileAndStoreExpression(resolver.GetExpression(resolutionInfo), typeInfo);

            throw new ResolutionFailedException(typeInfo.Type.FullName);
        }

        private object CompileAndStoreExpression(Expression expression, TypeInformation typeInformation)
        {
            var factory = Expression.Lambda<Func<object>>(expression).Compile();
            this.containerContext.DelegateRepository.AddOrUpdate(typeInformation.Type, factory);
            return factory();
        }
    }
}
