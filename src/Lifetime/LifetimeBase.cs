using Stashbox.BuildUp;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a lifetime manager.
    /// </summary>
    public abstract class LifetimeBase : ILifetime
    {
        /// <inheritdoc />
        public virtual Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder,
            ResolutionContext resolutionContext, Type resolveType) => objectBuilder.GetExpression(containerContext, serviceRegistration, resolutionContext, resolveType);

        /// <inheritdoc />
        public abstract ILifetime Create();
    }
}
