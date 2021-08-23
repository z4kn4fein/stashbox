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

        internal bool TypeMapIsValid(out string error)
        {
            error = null;
            if (this.ImplementationType == null)
            {
                error = $"Implementation type not set.";
                return false;
            }

            if (this.Context.Factory != null)
                return true;

            if (!this.ImplementationType.IsResolvableType())
            {
                error = $"The type {this.ImplementationType} could not be resolved. It's probably an interface, abstract class or primitive type.";
                return false;
            }

            if (this.ImplementationType.Implements(this.ServiceType)) return true;

            error = $"The type {this.ImplementationType} does not implement the service type {this.ServiceType}.";
            return false;

        }

        internal bool ImplementationIsResolvable(out string error)
        {
            error = null;
            if (this.Context.Factory != null)
                return true;

            if (this.ImplementationType.IsResolvableType()) return true;

            error = $"The type {this.ImplementationType} could not be resolved. It's probably an interface, abstract class or primitive type.";
            return false;

        }
    }
}
