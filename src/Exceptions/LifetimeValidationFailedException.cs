using System;

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents the exception the container throws when the lifetime validation is failed.
    /// </summary>
    public class LifetimeValidationFailedException : ExceptionBaseException
    {
        /// <summary>
        /// Constructs a <see cref="LifetimeValidationFailedException"/>.
        /// </summary>
        /// <param name="type">The type of the service.</param>
        /// <param name="message">The exception message.</param>
        public LifetimeValidationFailedException(Type type, string message)
            : base(type, message)
        { }
    }
}
