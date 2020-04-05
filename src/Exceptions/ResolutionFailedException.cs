using System;

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents the exception the container throws when a service resolution is failed.
    /// </summary>
    public class ResolutionFailedException : ExceptionBaseException
    {
        /// <summary>
        /// Constructs a <see cref="ResolutionFailedException"/>
        /// </summary>
        /// <param name="type">The type of the service.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ResolutionFailedException(Type type, string message = "Service is not registered or unresolvable type requested.", Exception innerException = null)
            : base(type, $"Could not resolve type {type.FullName}.{Environment.NewLine}{message}", innerException)
        { }
    }
}