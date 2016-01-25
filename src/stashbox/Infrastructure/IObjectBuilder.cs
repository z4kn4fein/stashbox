
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
        /// Creates an instance of a registered service.
        /// </summary>
        /// <param name="resolutionInfo">The info about the actual resolution.</param>
        /// <param name="resolveType">The type info about the resolved type.</param>
        /// <returns>The created object.</returns>
        object BuildInstance(ResolutionInfo resolutionInfo, TypeInformation resolveType);

        /// <summary>
        /// Creates the expression for creating an instance of a registered service.
        /// </summary>
        /// <param name="resolutionInfo">The info about the actual resolution.</param>
        /// <param name="resolutionInfoExpression">The expression of the info about the actual resolution.</param>
        /// <param name="resolveType">The type info about the resolved type.</param>
        /// <returns>The created object.</returns>
        Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType);

        /// <summary>
        /// Indicates a service updated event when a ReMap occures on a tracked service.
        /// </summary>
        /// <param name="registrationInfo">The new service registration.</param>
        void ServiceUpdated(RegistrationInfo registrationInfo);

        /// <summary>
        /// Cleans up the object builder.
        /// </summary>
        void CleanUp();
    }
}
