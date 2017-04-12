using System;

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents the exception threw when a service resolution failed.
    /// </summary>
    public class ResolutionFailedException : ExceptionBase
    {
        /// <summary>
        /// Constructs a <see cref="ResolutionFailedException"/>
        /// </summary>
        /// <param name="type">The type of the service.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ResolutionFailedException(Type type, string message = "service not registered or unresolvable type requested.", Exception innerException = null)
            : base(type, $"Could not resolve type {type.FullName}, reason: {message}", innerException)
        { }
    }
}