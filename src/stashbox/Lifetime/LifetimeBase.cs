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
        public virtual bool IsTransient => false;

        /// <inheritdoc />
        public virtual bool IsScoped => false;

        /// <inheritdoc />
        public virtual Expression GetExpression(IContainerContext containerContext, IObjectBuilder objectBuilder,
            ResolutionInfo resolutionInfo, TypeInformation resolveType) =>
                objectBuilder.GetExpression(resolutionInfo, resolveType);

        /// <inheritdoc />
        public abstract ILifetime Create();

        /// <inheritdoc />
        public virtual void CleanUp() { }
    }
}
