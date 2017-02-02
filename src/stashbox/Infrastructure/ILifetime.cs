
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
        /// Indicates that this life time manager holds or doesn't hold any reference to the resolved object.
        /// </summary>
        bool IsTransient { get; }

        /// <summary>
        /// Gets the instance managed by the <see cref="ILifetime"/>
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="objectBuilder">An <see cref="IObjectBuilder"/> implementation.</param>
        /// <param name="resolutionInfo">The info about the actual resolution.</param>
        /// <param name="resolveType">The type info about the resolved type.</param>
        /// <returns>The lifetime managed object.</returns>
        object GetInstance(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType);

        /// <summary>
        /// Gets the expression for getting the instance managed by the <see cref="ILifetime"/>
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="objectBuilder">An <see cref="IObjectBuilder"/> implementation.</param>
        /// <param name="resolutionInfo">The info about the actual resolution.</param>
        /// <param name="resolveType">The type info about the resolved type.</param>
        /// <returns>The lifetime managed object.</returns>
        Expression GetExpression(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType);

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <returns>The new life time manager instance.</returns>
        ILifetime Create();

        /// <summary>
        /// Cleans up the lifetime manager.
        /// </summary>
        void CleanUp();
    }
}
