using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
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
            ResolutionInfo resolutionInfo, Type resolveType) => objectBuilder.GetExpression(containerContext, serviceRegistration, resolutionInfo, resolveType);

        /// <inheritdoc />
        public abstract ILifetime Create();
    }
}
