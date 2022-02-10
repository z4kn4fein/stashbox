using System;

namespace Stashbox.Registration
{
    /// <summary>
    /// Details about a registration.
    /// </summary>
    public readonly struct RegistrationDiagnosticsInfo
    {
        /// <summary>
        /// The service type.
        /// </summary>
        public readonly Type ServiceType;

        /// <summary>
        /// The implementation type.
        /// </summary>
        public readonly Type ImplementationType;

        /// <summary>
        /// The registration name.
        /// </summary>
        public readonly object Name;

        /// <summary>
        /// Constructs a <see cref="RegistrationDiagnosticsInfo"/>.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="name">The registration name.</param>
        public RegistrationDiagnosticsInfo(Type serviceType, Type implementationType, object name) : this()
        {
            this.ServiceType = serviceType;
            this.ImplementationType = implementationType;
            this.Name = name;
        }

        /// <summary>
        /// The string representation of the registration.
        /// </summary>
        /// <returns>The string representation of the registration.</returns>
        public override string ToString() => $"{this.ServiceType.GetDiagnosticsView()} => {this.ImplementationType.GetDiagnosticsView()}, name: {this.Name ?? "null"}";
    }
}
