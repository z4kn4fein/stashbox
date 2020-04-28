using System;

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents the exception the container throws when a circular dependency is found.
    /// </summary>
    public class CircularDependencyException : ExceptionBaseException
    {
        /// <summary>
        /// Constructs a <see cref="CircularDependencyException"/>.
        /// </summary>
        /// <param name="type">The type of the service type.</param>
        /// <param name="innerException">The inner exception.</param>
        public CircularDependencyException(Type type, Exception innerException = null)
            : base(type, $"Circular dependency detected during the resolution of {type.FullName}.", innerException)
        {
        }
    }
}
