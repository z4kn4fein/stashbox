﻿using Stashbox.Resolution;
using System;
using System.Collections.Generic;

namespace Stashbox.Registration;

/// <summary>
/// Represents a decorator registration repository.
/// </summary>
public interface IDecoratorRepository
{
    /// <summary>
    /// Adds a decorator to the repository.
    /// </summary>
    /// <param name="type">The decorated type.</param>
    /// <param name="serviceRegistration">The decorator registration.</param>
    /// <param name="remap">If true, all the registrations mapped to a service type will be replaced.</param>
    void AddDecorator(Type type, ServiceRegistration serviceRegistration, bool remap);

    /// <summary>
    /// Gets all decorator registration.
    /// </summary>
    /// <param name="implementationTypeToDecorate">The implementation type to decorate.</param>
    /// <param name="typeInformation">The info about the decorated type.</param>
    /// <param name="resolutionContext">The resolution context.</param>
    /// <returns>The decorator registrations if any exists, otherwise null.</returns>
    IEnumerable<ServiceRegistration>? GetDecoratorsOrDefault(Type implementationTypeToDecorate, TypeInformation typeInformation, ResolutionContext resolutionContext);

    /// <summary>
    /// Returns all registration mappings.
    /// </summary>
    /// <returns>The registration mappings.</returns>
    IEnumerable<KeyValuePair<Type, ServiceRegistration>> GetRegistrationMappings();
}