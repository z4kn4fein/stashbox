using System;
using System.Linq.Expressions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Resolution;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a per resolution request lifetime.
    /// </summary>
    public class ResolutionRequestLifetime : ScopedLifetimeBase
    {
        /// <inheritdoc />
        public override ILifetime Create() => new ResolutionRequestLifetime();

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration,
            IObjectBuilder objectBuilder, ResolutionContext resolutionContext, Type resolveType)
        {
            var instance = resolutionContext.GetPerRequestItemOrDefault(base.ScopeId);
            if (instance != null)
                return Expression.Constant(instance);

            var factory = base.GetFactoryDelegate(containerContext, serviceRegistration, objectBuilder,
                resolutionContext, resolveType);

            if (factory == null)
                return null;

            instance = factory(resolutionContext.ResolutionScope);

            resolutionContext.AddPerRequestItem(base.ScopeId, instance);
            return Expression.Constant(instance);
        }
    }
}
