using Stashbox.Lifetime;
using Stashbox.Registration.ServiceRegistrations;
using Stashbox.Utils;
using System;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// Represents the collected configuration options of a registration.
    /// </summary>
    public class RegistrationConfiguration
    {
        /// <summary>
        /// The service type.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// The implementation type.
        /// </summary>
        public Type ImplementationType => this.Registration.ImplementationType;

        internal ServiceRegistration Registration { get; set; }

        internal RegistrationConfiguration(Type serviceType, Type implementationType, object? name,
            LifetimeDescriptor lifetimeDescriptor, bool isDecorator)
        {
            this.ServiceType = serviceType;
            this.Registration = new ServiceRegistration(implementationType, name, lifetimeDescriptor, isDecorator);
        }

        internal void ValidateTypeMap()
        {
            if (this.Registration is FactoryRegistration)
                return;

            Shield.EnsureTypeMapIsValid(this.ServiceType, this.ImplementationType);
        }

        internal void ValidateImplementationIsResolvable()
        {
            if (this.Registration is FactoryRegistration)
                return;

            Shield.EnsureIsResolvable(this.ImplementationType);
        }
    }
}
