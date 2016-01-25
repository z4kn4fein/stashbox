using Stashbox.Entity;
using Stashbox.Infrastructure;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a transient lifetime manager.
    /// </summary>
    public class TransientLifetime : ILifetime
    {
        /// <summary>
        /// Gets the instance managed by the <see cref="TransientLifetime"/>
        /// </summary>
        /// <param name="objectBuilder">An <see cref="IObjectBuilder"/> implementation.</param>
        /// <param name="resolutionInfo">The info about the actual resolution.</param>
        /// <param name="resolveType">The type info about the resolved type.</param>
        /// <returns>The lifetime managed object.</returns>
        public object GetInstance(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            return objectBuilder.BuildInstance(resolutionInfo, resolveType);
        }

        /// <summary>
        /// Gets the expression for getting the instance managed by the <see cref="TransientLifetime"/>
        /// </summary>
        /// <param name="objectBuilder">An <see cref="IObjectBuilder"/> implementation.</param>
        /// <param name="resolutionInfo">The info about the actual resolution.</param>
        /// <param name="resolutionInfoExpression">The expression of the info about the actual resolution.</param>
        /// <param name="resolveType">The type info about the resolved type.</param>
        /// <returns>The lifetime managed object.</returns>
        public Expression GetExpression(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            return objectBuilder.GetExpression(resolutionInfo, resolutionInfoExpression, resolveType);
        }

        /// <summary>
        /// Cleans up the lifetime manager.
        /// </summary>
        public void CleanUp()
        {
        }
    }
}
