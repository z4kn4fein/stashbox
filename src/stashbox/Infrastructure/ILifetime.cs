using Stashbox.Infrastructure.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a lifetime.
    /// </summary>
    public interface ILifetime
    {
        /// <summary>
        /// Gets the expression for getting the instance managed by the <see cref="ILifetime"/>
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="serviceRegistration">The service registration.</param>
        /// <param name="objectBuilder">An <see cref="IObjectBuilder"/> implementation.</param>
        /// <param name="resolutionContext">The info about the actual resolution.</param>
        /// <param name="resolveType">The requested type.</param>
        /// <returns>The lifetime managed object.</returns>
        Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder, ResolutionContext resolutionContext, Type resolveType);

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <returns>The new life time manager instance.</returns>
        ILifetime Create();
    }
}
