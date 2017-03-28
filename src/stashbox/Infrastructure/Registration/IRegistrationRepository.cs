using System;
using System.Collections.Generic;
using Stashbox.Entity;

namespace Stashbox.Infrastructure.Registration
{
    /// <summary>
    /// Represents a registration repository.
    /// </summary>
    public interface IRegistrationRepository
    {
        /// <summary>
        /// Adds or updates an element in the repository.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name of the registration.</param>
        /// <param name="canUpdate">Indicates that update is allowed</param>
        /// <param name="registration">The registration.</param>
        void AddOrUpdateRegistration(Type type, string name, bool canUpdate, IServiceRegistration registration);

        /// <summary>
        /// Retrieves a registration.
        /// </summary>
        /// <param name="type">The requested type.</param>
        /// <param name="name">The requested name.</param>
        /// <returns>The registration or null, if it doesn't exist.</returns>
        IServiceRegistration GetRegistrationOrDefault(Type type, string name = null);

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
        /// <param name="type">The requested type.</param>
        /// <returns>The registrations or null, if it doesn't exist.</returns>
        IEnumerable<IServiceRegistration> GetRegistrationsOrDefault(Type type);

        /// <summary>
        /// Retrieves all registrations.
        /// </summary>
        /// <returns>The registrations.</returns>
        IEnumerable<IServiceRegistration> GetAllRegistrations();

        /// <summary>
        /// Check a type exists with conditions.
        /// </summary>
        /// <param name="type">The requested type.</param>
        /// <param name="name">The requested name.</param>
        /// <returns>True if the registration found, otherwise false.</returns>
        bool ContainsRegistration(Type type, string name);
    }
}
