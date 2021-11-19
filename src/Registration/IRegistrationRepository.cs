using Stashbox.Resolution;
using System;
using System.Collections.Generic;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a registration repository.
    /// </summary>
    public interface IRegistrationRepository
    {
        /// <summary>
        /// Adds or updates an element in the repository.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <param name="serviceType">The service type of the registration. Used as the key for the registration mapping.</param>
        /// <returns>True when the repository changed, otherwise false.</returns>
        bool AddOrUpdateRegistration(ServiceRegistration registration, Type serviceType);

        /// <summary>
        /// Remaps all the registrations mapped to a service type.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <param name="serviceType">The service type of the registration. Used as the key for the registration mapping.</param>
        /// <returns>True when the repository changed, otherwise false.</returns>
        bool AddOrReMapRegistration(ServiceRegistration registration, Type serviceType);

        /// <summary>
        /// Returns a registration.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <returns>The registration or null, if it doesn't exist.</returns>
        ServiceRegistration GetRegistrationOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext);

        /// <summary>
        /// Returns all registrations for a type.
        /// </summary>
        /// <param name="typeInfo">The requested type.</param>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <returns>The registrations or null, if it doesn't exist.</returns>
        IEnumerable<ServiceRegistration> GetRegistrationsOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext);

        /// <summary>
        /// Returns all registration mappings.
        /// </summary>
        /// <returns>The registration mappings.</returns>
        IEnumerable<KeyValuePair<Type, ServiceRegistration>> GetRegistrationMappings();

        /// <summary>
        /// Checks whether a type is registered in the repository.
        /// </summary>
        /// <param name="type">The requested type.</param>
        /// <param name="name">The requested name.</param>
        /// <param name="includeOpenGenerics">Determines whether open generic registrations should be taken into account when the given type is closed generic.</param>
        /// <returns>True if the registration found, otherwise false.</returns>
        bool ContainsRegistration(Type type, object name, bool includeOpenGenerics = true);
    }
}
