using Stashbox.Entity;
using System;
using System.Collections.Generic;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a registration repository.
    /// </summary>
    public interface IRegistrationRepository
    {
        /// <summary>
        /// Adds or updates an element in the repository.
        /// </summary>
        /// <param name="typeKey">The key type.</param>
        /// <param name="nameKey">The name of the registration.</param>
        /// <param name="canUpdate">Indicates that update is allowed</param>
        /// <param name="registration">The registration.</param>
        void AddOrUpdateRegistration(Type typeKey, string nameKey, bool canUpdate, IServiceRegistration registration);

        /// <summary>
        /// Retrieves a registration.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="checkConditions">Indicates that the operation should check the registration conditions.</param>
        /// <returns>The registration or null, if it doesn't exist.</returns>
        IServiceRegistration GetRegistrationOrDefault(TypeInformation typeInfo, bool checkConditions = false);

        /// <summary>
        /// Retrieves all registrations for a type.
        /// </summary>
        /// <param name="typeInfo">The requested type information.</param>
        /// <returns>The registrations or null, if it doesn't exist.</returns>
        IEnumerable<IServiceRegistration> GetRegistrationsOrDefault(TypeInformation typeInfo);

        /// <summary>
        /// Retrieves all registrations.
        /// </summary>
        /// <returns>The registrations.</returns>
        IEnumerable<IServiceRegistration> GetAllRegistrations();

        /// <summary>
        /// Check a type exists with conditions.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>True if the registration found, otherwise false.</returns>
        bool ContainsRegistration(TypeInformation typeInfo);

        /// <summary>
        /// Cleans up the repository.
        /// </summary>
        void CleanUp();
    }
}
