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
                var factory = this.containerContext.DelegateRepository.GetDelegateCacheOrDefault(typeInfo) ??
                    this.containerContext.DelegateRepository.GetWrapperDelegateCacheOrDefault(typeInfo);
                if (factory != null)
                    return factory();
            }

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
