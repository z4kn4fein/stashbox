using System;

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents the exception which will be threw when a circular dependency found.
    /// </summary>
    public class CircularDependencyException : ExceptionBase
    {
        /// <summary>
        /// Constructs a <see cref="CircularDependencyException"/>
        /// </summary>
        /// <param name="typeName">The name of the service type.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CircularDependencyException(string typeName, string message = null, Exception innerException = null)
            : base(typeName, message, innerException)
        {
        }
    }
}
