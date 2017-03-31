using System;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System.Linq.Expressions;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a lifetime manager.
    /// </summary>
    public abstract class LifetimeBase : ILifetime
    {
        /// <inheritdoc />
        public virtual Expression GetExpression(IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder,
            ResolutionInfo resolutionInfo, Type resolveType) => objectBuilder.GetExpression(serviceRegistration, resolutionInfo, resolveType);

        /// <inheritdoc />
        public abstract ILifetime Create();
    }
}
