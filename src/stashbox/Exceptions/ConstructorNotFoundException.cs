using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents a constructor not found exception.
    /// </summary>
    public class ConstructorNotFoundException : Exception
    {
        /// <summary>
        /// Constructs a <see cref="ConstructorNotFoundException"/>.
        /// </summary>
        /// <param name="type">The type on the constructor was not found.</param>
        /// <param name="argumentsLength">The length of the arguments.</param>
        /// <param name="innerException">The inner exception</param>
        public ConstructorNotFoundException(Type type, int argumentsLength, Exception innerException = null) :
            base($"Constructor not found for {type.FullName} with {argumentsLength} arguments.", innerException)
        {
        }
    }
}
