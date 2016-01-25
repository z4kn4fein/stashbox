
using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a lifetime.
    /// </summary>
    public interface ILifetime
    {
        /// <summary>
        /// Gets the instance managed by the <see cref="ILifetime"/>
        /// </summary>
        /// <param name="objectBuilder">An <see cref="IObjectBuilder"/> implementation.</param>
        /// <param name="resolutionInfo">The info about the actual resolution.</param>
        /// <param name="resolveType">The type info about the resolved type.</param>
        /// <returns>The lifetime managed object.</returns>
        object GetInstance(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType);

        /// <summary>
        /// Gets the expression for getting the instance managed by the <see cref="ILifetime"/>
        /// </summary>
        /// <param name="objectBuilder">An <see cref="IObjectBuilder"/> implementation.</param>
        /// <param name="resolutionInfo">The info about the actual resolution.</param>
        /// <param name="resolutionInfoExpression">The expression of the info about the actual resolution.</param>
        /// <param name="resolveType">The type info about the resolved type.</param>
        /// <returns>The lifetime managed object.</returns>
        Expression GetExpression(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType);

        /// <summary>
        /// Cleans up the lifetime manager.
        /// </summary>
        void CleanUp();
    }
}
