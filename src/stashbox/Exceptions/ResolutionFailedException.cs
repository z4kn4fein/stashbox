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
        /// <param name="typeName">The name of the service type.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ResolutionFailedException(string typeName, string message = null, Exception innerException = null)
            : base(typeName, $"Could not resolve type {typeName}, reason: {message}", innerException)
        { }
    }
}