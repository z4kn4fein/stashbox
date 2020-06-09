using System;
#if HAS_SERIALIZABLE
using System.Runtime.Serialization;
#endif

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents the exception the container throws when a circular dependency is found.
    /// </summary>
#if HAS_SERIALIZABLE
    [Serializable]
#endif
    public class CircularDependencyException : Exception
    {
        /// <summary>
        /// The type the container is currently resolving.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Constructs a <see cref="CircularDependencyException"/>.
        /// </summary>
        /// <param name="type">The type of the service type.</param>
        /// <param name="innerException">The inner exception.</param>
        public CircularDependencyException(Type type, Exception innerException = null)
            : base($"Circular dependency detected during the resolution of {type.FullName}.", innerException)
        {
            this.Type = type;
        }

#if HAS_SERIALIZABLE
        /// <inheritdoc />
        protected CircularDependencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
#endif
    }
}
