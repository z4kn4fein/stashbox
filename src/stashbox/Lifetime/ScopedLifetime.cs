using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a scoped lifetime.
    /// </summary>
    public class ScopedLifetime : ScopedLifetimeBase
    {
        /// <inheritdoc />
        public override ILifetime Create() => new ScopedLifetime();

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder, ResolutionContext resolutionContext, Type resolveType)
        {
            var factory = base.GetFactoryDelegate(containerContext, serviceRegistration, objectBuilder, resolutionContext, resolveType);
            if (factory == null)
                return null;

            return Expression.Convert(Expression.Call(resolutionContext.CurrentScopeParameter, Constants.GetOrAddScopedItemMethod, Expression.Constant(base.ScopeId), Expression.Constant(factory)), resolveType);
        }
    }
}
