using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a service registration.
    /// </summary>
    public interface IServiceRegistration
    {
        /// <summary>
        /// Gets the resolved instance.
        /// </summary>
        /// <param name="resolutionInfo">The info about the current resolution.</param>
        /// <param name="resolveType">The resolve type.</param>
        /// <returns>The created object.</returns>
        object GetInstance(ResolutionInfo resolutionInfo, TypeInformation resolveType);

        /// <summary>
        /// Creates an expression for creating the resolved instance.
        /// </summary>
        /// <param name="resolutionInfo">The info about the current resolution.</param>
        /// <param name="resolutionInfoExpression">The expression of the info about the current resolution.</param>
        /// <param name="resolveType">The resolve type.</param>
        /// <returns>The expression.</returns>
        Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType);

        /// <summary>
        /// Checks whether the registration can be used for a current resolution.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>True if the registration can be used for the current resolution, otherwise false.</returns>
        bool IsUsableForCurrentContext(TypeInformation typeInfo);

        /// <summary>
        /// True if the registration contains any condition, otherwise false.
        /// </summary>
        bool HasCondition { get; }

        /// <summary>
        /// Indicates a service updated event when a ReMap occures on a tracked service.
        /// </summary>
        /// <param name="registrationInfo">The new service registration.</param>
        void ServiceUpdated(RegistrationInfo registrationInfo);

        /// <summary>
        /// Cleans up the registration.
        /// </summary>
        void CleanUp();
    }
}
