using System;

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents an exception which is thrown when a named scope is not found for a registration with named scope lifetime.
    /// </summary>
    public class NamedScopeNotFoundException : ExceptionBaseException
    {
        /// <summary>
        /// Constructs a <see cref="NamedScopeNotFoundException"/>.
        /// </summary>
        /// <param name="type">The type which was tried to resolve.</param>
        /// <param name="scopeName">The name of the scope.</param>
        /// <param name="innerException">The inner exception.</param>
        public NamedScopeNotFoundException(Type type, object scopeName, Exception innerException = null)
            : base(type, $"Named scope '{scopeName}' not found for type {type.FullName} resolution.", innerException)
        {
        }
    }
}
