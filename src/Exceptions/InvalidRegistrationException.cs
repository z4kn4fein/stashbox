using System;
#if HAS_SERIALIZABLE
using System.Runtime.Serialization;
#endif

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents an exception the container throws when it detects an invalid registration.
    /// </summary>
#if HAS_SERIALIZABLE
    [Serializable]
#endif
    public class InvalidRegistrationException : Exception
    {
        /// <summary>
        /// The type the container is trying to register.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Constructs a <see cref="InvalidRegistrationException"/>.
        /// </summary>
        /// <param name="type">The type of the service.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidRegistrationException(Type type, string message, Exception innerException = null)
            : base($"Invalid registration with type {type.FullName}. Details: {message}", innerException)
        {
            this.Type = type;
        }

#if HAS_SERIALIZABLE
        /// <inheritdoc />
        protected InvalidRegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
#endif
    }
}
