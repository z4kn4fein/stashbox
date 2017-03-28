using System;
using Stashbox.Entity;
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
        /// <param name="resolutionInfo">The info about the actual resolution.</param>
        /// <param name="resolveType">The requested type.</param>
        /// <returns>The created object.</returns>
        Expression GetExpression(ResolutionInfo resolutionInfo, Type resolveType);

        /// <summary>
        /// Indicates that the object builder is handling the disposal of the produced instance or not.
        /// </summary>
        bool HandlesObjectDisposal { get; }
    }
}
