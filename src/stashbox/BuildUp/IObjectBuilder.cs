using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    /// <summary>
    /// Represents an object builder.
    /// </summary>
    public interface IObjectBuilder
    {
        /// <summary>
        /// Creates the expression for creating an instance of a registered service.
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="serviceRegistration">The service registration.</param>
        /// <param name="resolutionContext">The info about the actual resolution.</param>
        /// <param name="resolveType">The requested type.</param>
        /// <returns>The created object.</returns>
        Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType);

        /// <summary>
        /// Indicates that the object builder is handling the disposal of the produced instance or not.
        /// </summary>
        bool HandlesObjectLifecycle { get; }

        /// <summary>
        /// Produces an <see cref="IObjectBuilder"/>, when an implementor .
        /// </summary>
        /// <returns>The <see cref="IObjectBuilder"/> instance.</returns>
        IObjectBuilder Produce();
    }
}
