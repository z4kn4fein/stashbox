using System;

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents a missing method exception.
    /// </summary>
    public class MethodNotFoundException : ExceptionBase
    {
        /// <summary>
        /// The method name.
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// Constructs a <see cref="MethodNotFoundException"/>.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <param name="methodName">The method name.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public MethodNotFoundException(string typeName, string methodName, string message = null, Exception innerException = null) 
            : base(typeName, message, innerException)
        {
            this.MethodName = methodName;
        }
    }
}
