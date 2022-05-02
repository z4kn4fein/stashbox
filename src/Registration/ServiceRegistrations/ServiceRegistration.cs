using Stashbox.Lifetime;
using System;
using System.Threading;

namespace Stashbox.Registration.ServiceRegistrations
{
    /// <summary>
    /// Represents a service registration.
    /// </summary>
    public class ServiceRegistration
    {
        private static int GlobalRegistrationId;

        private static int GlobalRegistrationOrder;

        /// <summary>
        /// The registration id.
        /// </summary>
        public readonly int RegistrationId;

        /// <summary>
        /// True if the registration is a decorator.
        /// </summary>
        public readonly bool IsDecorator;

        /// <summary>
        /// Name of the registration.
        /// </summary>
        public object? Name { get; internal set; }

        /// <summary>
        /// Lifetime of the registration.
        /// </summary>
        public LifetimeDescriptor Lifetime { get; internal set; }

        /// <summary>
        /// The registration order indicator.
        /// </summary>
        public int RegistrationOrder { get; private set; }

        /// <summary>
        /// The implementation type.
        /// </summary>
        public Type ImplementationType { get; internal set; }

        internal ServiceRegistration(Type implementationType, object? name,
            LifetimeDescriptor lifetimeDescriptor, bool isDecorator)
        {
            this.ImplementationType = implementationType;
            this.IsDecorator = isDecorator;
            this.Name = name;
            this.Lifetime = lifetimeDescriptor;
            this.RegistrationId = ReserveRegistrationId();
            this.RegistrationOrder = ReserveRegistrationOrder();
        }

        internal ServiceRegistration(Type implementationType, object? name,
            LifetimeDescriptor lifetimeDescriptor, int registrationId, int registrationOrder, bool isDecorator)
        {
            this.ImplementationType = implementationType;
            this.IsDecorator = isDecorator;
            this.Name = name;
            this.Lifetime = lifetimeDescriptor;
            this.RegistrationId = registrationId;
            this.RegistrationOrder = registrationOrder;
        }

        internal void Replaces(ServiceRegistration serviceRegistration) =>
            RegistrationOrder = serviceRegistration.RegistrationOrder;

        private static int ReserveRegistrationId() =>
            Interlocked.Increment(ref GlobalRegistrationId);

        private static int ReserveRegistrationOrder() =>
            Interlocked.Increment(ref GlobalRegistrationOrder);
    }
}
