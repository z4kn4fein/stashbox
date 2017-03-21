using System;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a lifetime manager.
    /// </summary>
    public abstract class LifetimeBase : ILifetime
    {
        /// <inheritdoc />
        public virtual bool HandlesObjectDisposal => false;

        /// <inheritdoc />
        public virtual Expression GetExpression(IContainerContext containerContext, IObjectBuilder objectBuilder,
            ResolutionInfo resolutionInfo, Type resolveType) =>
                objectBuilder.GetExpression(resolutionInfo, resolveType);

        /// <inheritdoc />
        public abstract ILifetime Create();

        /// <inheritdoc />
        public virtual void CleanUp() { }
    }
}
