using System;

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents an exception the container throws when it detects an invalid registration.
    /// </summary>
    public class InvalidRegistrationException : ExceptionBaseException
    {
        /// <summary>
        /// Constructs a <see cref="InvalidRegistrationException"/>.
        /// </summary>
        /// <param name="type">The type of the service.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidRegistrationException(Type type, string message, Exception innerException = null)
            : base(type, $"Invalid registration with type {type.FullName}. Details: {message}", innerException)
        {
        }
    }
}
