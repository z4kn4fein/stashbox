using Stashbox.Entity;
using Stashbox.Infrastructure;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a transient lifetime manager.
    /// </summary>
    public class TransientLifetime : LifetimeBase
    {
        /// <inheritdoc />
        public override object GetInstance(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            return base.GetInstance(containerContext, objectBuilder, resolutionInfo, resolveType);
        }

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            return base.GetExpression(containerContext, objectBuilder, resolutionInfo, resolutionInfoExpression, resolveType);
        }
    }
}
