using Stashbox.Expressions;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a lifetime descriptor which applies to factory delegates.
    /// </summary>
    public abstract class FactoryLifetimeDescriptor : LifetimeDescriptor
    {
        private protected override Expression BuildLifetimeAppliedExpression(ExpressionBuilder expressionBuilder, IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type resolveType)
        {
            var factory = GetFactoryDelegateForRegistration(expressionBuilder, serviceRegistration, resolutionContext, resolveType);
            return factory == null ? null : this.ApplyLifetime(factory, serviceRegistration, resolutionContext, resolveType);
        }

        private static Func<IResolutionScope, object> GetFactoryDelegateForRegistration(ExpressionBuilder expressionBuilder,
            IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (!IsRegistrationOutputCacheable(serviceRegistration, resolutionContext))
                return GetNewFactoryDelegate(expressionBuilder, serviceRegistration, resolutionContext.BeginSubGraph(), resolveType);

            var factory = resolutionContext.GetCachedFactory(serviceRegistration.RegistrationId);
            if (factory != null)
                return factory;

            factory = GetNewFactoryDelegate(expressionBuilder, serviceRegistration, resolutionContext.BeginSubGraph(), resolveType);
            resolutionContext.CacheFactory(serviceRegistration.RegistrationId, factory);
            return factory;
        }

        private static Func<IResolutionScope, object> GetNewFactoryDelegate(ExpressionBuilder expressionBuilder, IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type resolveType) =>
            GetExpressionForRegistration(expressionBuilder, serviceRegistration, resolutionContext, resolveType)
                ?.CompileDelegate(resolutionContext, resolutionContext.CurrentContainerContext.ContainerConfiguration);

        /// <summary>
        /// Derived types are using this method to apply their lifetime to the instance creation.
        /// </summary>
        /// <param name="factory">The factory which can be used to instantiate the service.</param>
        /// <param name="serviceRegistration">The service registration.</param>
        /// <param name="resolutionContext">The info about the actual resolution.</param>
        /// <param name="resolveType">The type of the resolved service.</param>
        /// <returns>The lifetime managed expression.</returns>
        protected abstract Expression ApplyLifetime(Func<IResolutionScope, object> factory, IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type resolveType);
    }
}
