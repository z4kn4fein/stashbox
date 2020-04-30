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
        {
            this.ServiceType = serviceType;
            this.ImplementationType = implementationType;
            this.Context = RegistrationContext.New();
        }

        internal bool TypeMapIsValid(out string error)
        {
            error = null;
            if (this.Context.ExistingInstance != null ||
                this.Context.ContainerFactory != null ||
                this.Context.SingleFactory != null)
                return true;


            if (!this.ImplementationType.IsResolvableType())
            {
                error = $"The type {this.ImplementationType} could not be resolved. It's probably an interface, abstract class or primitive type.";
                return false;
            }

            if (!this.ImplementationType.Implements(this.ServiceType))
            {
                error = $"The type {this.ImplementationType} does not implement the service type {this.ServiceType}.";
                return false;
            }

            return true;
        }
    }
}
