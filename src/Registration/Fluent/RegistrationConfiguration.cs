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
        public Type ImplementationType { get; }

        internal RegistrationContext Context { get; }

        internal RegistrationConfiguration(Type serviceType, Type implementationType)
        {
            this.ServiceType = serviceType;
            this.ImplementationType = implementationType;
            this.Context = RegistrationContext.New();
        }
    }
}
