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
        /// Adds a new registration into the repository.
        /// </summary>
        /// <param name="typeKey">The key type.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="nameKey">The name of the registration.</param>
        void AddRegistration(Type typeKey, IServiceRegistration registration, string nameKey);

        /// <summary>
        /// Adds or updates an element in the repository.
        /// </summary>
        /// <param name="typeKey">The key type.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="nameKey">The name of the registration.</param>
        void AddOrUpdateRegistration(Type typeKey, IServiceRegistration registration, string nameKey);

        /// <summary>
        /// Adds a new generic definition into the repository.
        /// </summary>
        /// <param name="typeKey">The key type.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="nameKey">The name of the registration.</param>
        void AddGenericDefinition(Type typeKey, IServiceRegistration registration, string nameKey);

        /// <summary>
        /// Adds or updates a generic definition in the repository.
        /// </summary>
        /// <param name="typeKey">The key type.</param>
        /// <param name="registration">The registration.</param>
        /// <param name="nameKey">The name of the registration.</param>
        void AddOrUpdateGenericDefinition(Type typeKey, IServiceRegistration registration, string nameKey);

        /// <summary>
        /// Gets all registrations.
        /// </summary>
        /// <returns>A collection of the registrations.</returns>
        IEnumerable<IServiceRegistration> GetAllRegistrations();

        /// <summary>
        /// Tries to retrieve a registration with conditions.
        /// </summary>
        /// <param name="typeInfo">The requested type information.</param>
        /// <param name="registration">The retrieved registration.</param>
        /// <returns>True if a registration was found, otherwise false.</returns>
        bool TryGetRegistrationWithConditions(TypeInformation typeInfo, out IServiceRegistration registration);

        /// <summary>
        /// Tries to retrieve a non generic definition registration with conditions.
        /// </summary>
        /// <param name="typeInfo">The requested type information.</param>
        /// <param name="registration">The retrieved registration.</param>
        /// <returns>True if a registration was found, otherwise false.</returns>
        bool TryGetRegistrationWithConditionsWithoutGenericDefinitionExtraction(TypeInformation typeInfo,
            out IServiceRegistration registration);

        /// <summary>
        /// Tries to retrieve a registrations.
        /// </summary>
        /// <param name="typeInfo">The requested type information.</param>
        /// <param name="registration">The retrieved registration.</param>
        /// <returns>True if a registration was found, otherwise false.</returns>
        bool TryGetRegistration(TypeInformation typeInfo, out IServiceRegistration registration);

        /// <summary>
        /// Tries to retrieve all registrations for a type.
        /// </summary>
        /// <param name="typeInfo">The requested type information.</param>
        /// <param name="registrations">The retrieved registrations.</param>
        /// <returns>True if registrations were found, otherwise false.</returns>
        bool TryGetTypedRepositoryRegistrations(TypeInformation typeInfo, out IServiceRegistration[] registrations);
        
        /// <summary>
        /// Check a type exists with conditions.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>True if the registration found, otherwise false.</returns>
        bool ConstainsRegistrationWithConditions(TypeInformation typeInfo);

        /// <summary>
        /// Cleans up the repository.
        /// </summary>
        void CleanUp();
    }
}
