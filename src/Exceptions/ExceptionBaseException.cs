using System;

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents the base of the custom exceptions used by the <see cref="StashboxContainer"/>
    /// </summary>
    public class ExceptionBaseException : Exception
    {
        /// <summary>
        /// The actually resolved type.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Constructs a <see cref="ExceptionBaseException"/>.
        /// </summary>
        /// <param name="type">The name of the type.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ExceptionBaseException(Type type, string message = null, Exception innerException = null)
            : base(message, innerException)
        {
            this.Type = type;
        }
    }
}