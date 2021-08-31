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
        private protected override Expression BuildLifetimeAppliedExpression(ServiceRegistration serviceRegistration, 
            ResolutionContext resolutionContext, Type requestedType)
        {
            var factory = GetFactoryDelegateForRegistration(serviceRegistration, resolutionContext, requestedType);
            return factory == null ? null : this.ApplyLifetime(factory, serviceRegistration, resolutionContext, requestedType);
        }

        private static Func<IResolutionScope, object> GetFactoryDelegateForRegistration(ServiceRegistration serviceRegistration, 
            ResolutionContext resolutionContext, Type requestedType)
        {
            if (!IsRegistrationOutputCacheable(serviceRegistration, resolutionContext))
                return GetNewFactoryDelegate(serviceRegistration, resolutionContext.BeginSubGraph(), requestedType);

            var factory = resolutionContext.GetCachedFactory(serviceRegistration.RegistrationId);
            if (factory != null)
                return factory;

            factory = GetNewFactoryDelegate(serviceRegistration, resolutionContext.BeginSubGraph(), requestedType);
            resolutionContext.CacheFactory(serviceRegistration.RegistrationId, factory);
            return factory;
        }

        private static Func<IResolutionScope, object> GetNewFactoryDelegate(ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type requestedType) =>
            GetExpressionForRegistration(serviceRegistration, resolutionContext, requestedType)
                ?.CompileDelegate(resolutionContext, resolutionContext.CurrentContainerContext.ContainerConfiguration);

        /// <summary>
        /// Derived types are using this method to apply their lifetime to the instance creation.
        /// </summary>
        /// <param name="factory">The factory which can be used to instantiate the service.</param>
        /// <param name="serviceRegistration">The service registration.</param>
        /// <param name="resolutionContext">The info about the actual resolution.</param>
        /// <param name="resolveType">The type of the resolved service.</param>
        /// <returns>The lifetime managed expression.</returns>
        protected abstract Expression ApplyLifetime(Func<IResolutionScope, object> factory, ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type resolveType);
    }
}
