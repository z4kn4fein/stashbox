﻿using Stashbox.Entity;
using System;
using System.Collections.Generic;

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
        /// <param name="remap">If true, all the registrations mapped to a service type will be replaced.</param>
        /// <param name="replace">If true, only one existing registration will be replaced when multiple exists.</param>
        /// <param name="registration">The registration.</param>
        void AddOrUpdateRegistration(IServiceRegistration registration, bool remap, bool replace);

        /// <summary>
        /// Retrieves a registration.
        /// </summary>
        /// <param name="type">The requested type.</param>
        /// <param name="scopeNames">The scope names info.</param>
        /// <param name="name">The requested name.</param>
        /// <returns>The registration or null, if it doesn't exist.</returns>
        IServiceRegistration GetRegistrationOrDefault(Type type, ISet<string> scopeNames = null, object name = null);

        /// <summary>
        /// Retrieves a registration.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="scopeNames">The scope names info.</param>
        /// <returns>The registration or null, if it doesn't exist.</returns>
        IServiceRegistration GetRegistrationOrDefault(TypeInformation typeInfo, ISet<string> scopeNames);

        /// <summary>
        /// Retrieves all registrations for a type.
        /// </summary>
        /// <param name="type">The requested type.</param>
        /// <param name="scopeNames">The scope names info.</param>
        /// <returns>The registrations or null, if it doesn't exist.</returns>
        IEnumerable<KeyValue<object, IServiceRegistration>> GetRegistrationsOrDefault(Type type, ISet<string> scopeNames = null);

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
        bool ContainsRegistration(Type type, object name);
    }
}
