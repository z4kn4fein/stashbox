
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
        /// Indicates that the lifetime manager stores or doesn't store any reference to the resolved object.
        /// If it's set to true and the tracking for transient disposal objects is enabled, the registration will be copied between scopes and the output will be tracked for disposal.
        /// </summary>
        bool IsTransient { get; }

        /// <summary>
        /// Indicates that the registration which is registered with this lifetime should be copied between scopes and will produce a new instance in every scope.
        /// </summary>
        bool IsScoped { get; }

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
