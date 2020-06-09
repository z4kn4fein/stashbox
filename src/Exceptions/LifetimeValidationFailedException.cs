using System;
#if HAS_SERIALIZABLE
using System.Runtime.Serialization;
#endif

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents the exception the container throws when the lifetime validation is failed.
    /// </summary>
#if HAS_SERIALIZABLE
    [Serializable]
#endif
    public class LifetimeValidationFailedException : Exception
    {
        /// <summary>
        /// The type the container is currently resolving.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Constructs a <see cref="LifetimeValidationFailedException"/>.
        /// </summary>
        /// <param name="type">The type of the service.</param>
        /// <param name="message">The exception message.</param>
        public LifetimeValidationFailedException(Type type, string message)
            : base(message)
        {
            this.Type = type;
        }

#if HAS_SERIALIZABLE
        /// <inheritdoc />
        protected LifetimeValidationFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
#endif
    }
}
