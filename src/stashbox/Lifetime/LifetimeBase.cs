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
        public virtual Expression GetExpression(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            return objectBuilder.GetExpression(resolutionInfo, resolutionInfoExpression, resolveType);
        }

        /// <inheritdoc />
        public virtual object GetInstance(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            return objectBuilder.BuildInstance(resolutionInfo, resolveType);
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
