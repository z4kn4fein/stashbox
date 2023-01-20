using Stashbox.Lifetime;
using System;

namespace Stashbox.Registration.Fluent;

/// <summary>
/// Represents the fluent service registration api.
/// </summary>
public class UnknownRegistrationConfigurator : RegistrationConfigurator
{
    internal bool RegistrationShouldBeSkipped { get; private set; }

    internal UnknownRegistrationConfigurator(Type serviceType, Type implementationType, object? name,
        LifetimeDescriptor lifetimeDescriptor)
        : base(serviceType, implementationType, lifetimeDescriptor, name)
    { }

    /// <summary>
    /// Sets the current registration's implementation type.
    /// </summary>
    /// <param name="implementationType">The implementation type.</param>
    /// <returns>The fluent configurator.</returns>
    public UnknownRegistrationConfigurator SetImplementationType(Type implementationType)
    {
        if (!implementationType.Implements(this.ServiceType))
            throw new ArgumentException($"The type {implementationType} does not implement the actual service type {this.ServiceType}.");

        this.ImplementationType = implementationType;
        return this;

    }

    /// <summary>
    /// Marks the current unknown type registration as skipped.
    /// </summary>
    /// <returns>The fluent configurator.</returns>
    public UnknownRegistrationConfigurator Skip()
    {
        this.RegistrationShouldBeSkipped = true;
        return this;
    }
}