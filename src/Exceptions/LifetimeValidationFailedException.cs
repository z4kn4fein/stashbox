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
        /// <param name="innerException">The inner exception.</param>
        public LifetimeValidationFailedException(Type type, string message, Exception innerException = null)
            : base(type,
                $"{message}{Environment.NewLine}If you want to disable this error then use " +
                "the following container configuration: 'new StashboxContainer(c => c.DisableLifetimeValidation())'.",
                innerException)
        { }
    }
}
