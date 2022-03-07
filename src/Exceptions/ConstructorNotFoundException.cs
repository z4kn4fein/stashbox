using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents a constructor not found exception.
    /// </summary>
    [Serializable]
    public class ConstructorNotFoundException : Exception
    {
        /// <summary>
        /// Constructs a <see cref="ConstructorNotFoundException"/>.
        /// </summary>
        /// <param name="type">The type on the constructor was not found.</param>
        /// <param name="argumentTypes">The arguments.</param>
        /// <param name="innerException">The inner exception</param>
        public ConstructorNotFoundException(Type type, Type[] argumentTypes, Exception? innerException = null) :
            base($"Constructor not found for '{type.FullName}' with the given argument types: {argumentTypes.Select(t => t.FullName).Aggregate((t1, t2) => $"{t1}, {t2}")}.", innerException)
        { }

        /// <summary>
        /// Constructs a <see cref="ConstructorNotFoundException"/>.
        /// </summary>
        /// <param name="type">The type on the constructor was not found.</param>
        /// <param name="innerException">The inner exception</param>
        public ConstructorNotFoundException(Type type, Exception? innerException = null) :
            base($"Constructor not found for '{type.FullName}' with no arguments.", innerException)
        { }

        /// <summary>
        /// Constructs a <see cref="ConstructorNotFoundException"/>.
        /// </summary>
        /// <param name="type">The type on the constructor was not found.</param>
        /// <param name="argument">The argument type.</param>
        /// <param name="innerException">The inner exception</param>
        public ConstructorNotFoundException(Type type, Type argument, Exception? innerException = null) :
            base($"Constructor not found for '{type.FullName}' with the argument type: {argument.FullName}.", innerException)
        { }

        /// <inheritdoc />
        protected ConstructorNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
