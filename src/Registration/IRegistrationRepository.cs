using Stashbox.Entity;
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
        /// <param name="remap">If true, all the registrations mapped to a service type will be replaced.</param>
        /// <param name="replace">If true, only one existing registration will be replaced when multiple exists.</param>
        void AddOrUpdateRegistration(IServiceRegistration registration, Type serviceType, bool remap, bool replace);

        /// <summary>
        /// Retrieves a registration.
        /// </summary>
        /// <param name="type">The requested type.</param>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <param name="name">The requested name.</param>
        /// <returns>The registration or null, if it doesn't exist.</returns>
        IServiceRegistration GetRegistrationOrDefault(Type type, ResolutionContext resolutionContext, object name = null);

        /// <summary>
        /// Retrieves a registration.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <returns>The registration or null, if it doesn't exist.</returns>
        IServiceRegistration GetRegistrationOrDefault(TypeInformation typeInfo, ResolutionContext resolutionContext);

        /// <summary>
        /// Retrieves all registrations for a type.
        /// </summary>
        /// <param name="type">The requested type.</param>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <returns>The registrations or null, if it doesn't exist.</returns>
        IEnumerable<KeyValue<object, IServiceRegistration>> GetRegistrationsOrDefault(Type type, ResolutionContext resolutionContext);

        /// <summary>
        /// Retrieves all registration mappings.
        /// </summary>
        /// <returns>The registration mappings.</returns>
        IEnumerable<KeyValue<Type, IServiceRegistration>> GetRegistrationMappings();

        /// <summary>
        /// Check a type exists with conditions.
        /// </summary>
        /// <param name="type">The requested type.</param>
        /// <param name="name">The requested name.</param>
        /// <returns>True if the registration found, otherwise false.</returns>
        bool ContainsRegistration(Type type, object name);
    }
}
