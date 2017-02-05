using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a lifetime manager.
    /// </summary>
    public class LifetimeBase : ILifetime
    {
        /// <inheritdoc />
        public virtual bool IsTransient => false;

        /// <inheritdoc />
        public virtual Expression GetExpression(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            return objectBuilder.GetExpression(resolutionInfo, resolveType);
        }

        /// <inheritdoc />
        public virtual ILifetime Create()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual void CleanUp()
        { }
    }
}
