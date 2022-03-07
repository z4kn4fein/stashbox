using System;
using System.Runtime.Serialization;

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents the exception the container throws when a service resolution is failed.
    /// </summary>
    [Serializable]
    public class ResolutionFailedException : Exception
    {
        /// <summary>
        /// The type the container is currently resolving.
        /// </summary>
        public Type? Type { get; }

        /// <summary>
        /// Constructs a <see cref="ResolutionFailedException"/>.
        /// </summary>
        /// <param name="type">The type of the service.</param>
        /// <param name="name">The name of the service.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ResolutionFailedException(Type? type,
            object? name = null,
            string message = "Service is not registered properly or unresolvable type requested.",
            Exception? innerException = null)
            : base($"Unable to resolve type '{type?.FullName}'{(name != null ? " with the name \'" + name + "\'" : "")}.{Environment.NewLine}{message}", innerException)
        {
            this.Type = type;
        }

        /// <inheritdoc />
        protected ResolutionFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}