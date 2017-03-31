using System;
using Stashbox.Registration;

namespace Stashbox.Infrastructure.Registration
{
    /// <summary>
    /// Represents meta information of a service registration.
    /// </summary>
    public interface IRegistrationContextMeta
    {
        /// <summary>
        /// The service type.
        /// </summary>
        Type ServiceType { get; }

        /// <summary>
        /// The implementation type.
        /// </summary>
        Type ImplementationType { get; }

        /// <summary>
        /// The registration context.
        /// </summary>
        RegistrationContextData Context { get; }
    }
}
