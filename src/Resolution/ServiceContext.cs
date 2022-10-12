using Stashbox.Registration;
using System.Linq.Expressions;

namespace Stashbox.Resolution
{
    /// <summary>
    /// Represents the context of a service.
    /// </summary>
    public readonly struct ServiceContext
    {
        /// <summary>
        /// The expression that describes the instantiation of the service.
        /// </summary>
        public readonly Expression ServiceExpression;

        /// <summary>
        /// The registration of the service.
        /// </summary>
        public readonly ServiceRegistration? ServiceRegistration;

        /// <summary>
        /// Constructs a <see cref="ServiceContext"/>.
        /// </summary>
        /// <param name="serviceExpression">The expression that describes the instantiation of the service.</param>
        /// <param name="serviceRegistration">The registration of the service.</param>
        public ServiceContext(Expression serviceExpression, ServiceRegistration? serviceRegistration)
        {
            this.ServiceExpression = serviceExpression;
            this.ServiceRegistration = serviceRegistration;
        }

        internal bool IsEmpty() => this.Equals(Empty);

        private bool Equals(ServiceContext other) =>
            ReferenceEquals(ServiceExpression, other.ServiceExpression) && ReferenceEquals(ServiceRegistration, other.ServiceRegistration);

        internal static readonly ServiceContext Empty = default;
    }
}