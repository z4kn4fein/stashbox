using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a singleton lifetime manager.
    /// </summary>
    public class SingletonLifetime : LifetimeDescriptor
    {
        /// <inheritdoc />
        protected override Expression GetLifetimeAppliedExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, 
            ResolutionContext resolutionContext, Type resolveType)
        {
            var factory = base.GetFactoryDelegate(containerContext, serviceRegistration, resolutionContext, resolveType);
            if (factory == null)
                return null;

            return resolutionContext.CurrentContainerContext.Container.RootScope
                .GetOrAddScopedItem(serviceRegistration.RegistrationId, serviceRegistration.RegistrationName, factory).AsConstant();
        }
    }
}
