using Stashbox.Entity;
using Stashbox.Infrastructure.Registration;
using System;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents an object builder.
    /// </summary>
    public interface IObjectBuilder
    {
        /// <summary>
        /// Creates the expression for creating an instance of a registered service.
        /// </summary>
        /// <param name="serviceRegistration">The service registration.</param>
        /// <param name="resolutionInfo">The info about the actual resolution.</param>
        /// <param name="resolveType">The requested type.</param>
        /// <returns>The created object.</returns>
        Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType);

        /// <summary>
        /// Indicates that the object builder is handling the disposal of the produced instance or not.
        /// </summary>
        bool HandlesObjectLifecycle { get; }

        /// <summary>
        /// Produces an <see cref="IObjectBuilder"/>.
        /// </summary>
        /// <returns>The <see cref="IObjectBuilder"/> instance.</returns>
        IObjectBuilder Produce();
    }
}
