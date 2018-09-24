using Stashbox.BuildUp;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a singleton lifetime manager.
    /// </summary>
    public class SingletonLifetime : ScopedLifetimeBase
    {
        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder, ResolutionContext resolutionContext, Type resolveType)
        {
            var factory = base.GetFactoryDelegate(containerContext, serviceRegistration, objectBuilder, resolutionContext, resolveType);
            return factory == null ? null : resolutionContext.RootScope.GetOrAddScopedItem(base.ScopeId, factory).AsConstant();
        }
        /// <inheritdoc />
        public override ILifetime Create() => new SingletonLifetime();
    }
}
