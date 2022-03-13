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
        public Type ImplementationType { get; protected set; }

        internal RegistrationContext Context { get; }

        internal RegistrationConfiguration(Type serviceType, Type implementationType)
            : this(serviceType, implementationType, new RegistrationContext())
        { }

        internal RegistrationConfiguration(Type serviceType, Type implementationType, RegistrationContext context)
        {
            this.ServiceType = serviceType;
            this.ImplementationType = implementationType;
            this.Context = context;
        }

        internal void ValidateTypeMap()
        {
            if (this.Context.Factory != null)
                return;

            Shield.EnsureTypeMapIsValid(this.ServiceType, this.ImplementationType);
        }

        internal void ValidateImplementationIsResolvable()
        {
            if (this.Context.Factory != null)
                return;

            Shield.EnsureIsResolvable(this.ImplementationType);
        }
    }
}
