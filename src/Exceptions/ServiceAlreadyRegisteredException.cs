using Stashbox.Configuration;
using System;
#if HAS_SERIALIZABLE
using System.Runtime.Serialization;
#endif

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents the exception the container throws when a registration process fails due to service duplication.
    /// Occurs when the container is configured with <see cref="Rules.RegistrationBehavior.ThrowException"/>.
    /// </summary>
#if HAS_SERIALIZABLE
    [Serializable]
#endif
    public class ServiceAlreadyRegisteredException : Exception
    {
        /// <summary>
        /// The type the container is trying to register.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Constructs a <see cref="ServiceAlreadyRegisteredException"/>.
        /// </summary>
        /// <param name="type">The type of the service.</param>
        /// <param name="innerException">The inner exception.</param>
        public ServiceAlreadyRegisteredException(Type type, Exception innerException = null)
            : base($"The type {type.FullName} is already registered.", innerException)
        {
            this.Type = type;
        }

#if HAS_SERIALIZABLE
        /// <inheritdoc />
        protected ServiceAlreadyRegisteredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
#endif
    }
}
