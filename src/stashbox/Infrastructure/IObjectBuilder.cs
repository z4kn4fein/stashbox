
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
        /// <param name="resolveType">The type info about the resolved type.</param>
        /// <returns>The created object.</returns>
        Expression GetExpression(ResolutionInfo resolutionInfo, TypeInformation resolveType);

        /// <summary>
        /// Cleans up the object builder.
        /// </summary>
        void CleanUp();
    }
}
