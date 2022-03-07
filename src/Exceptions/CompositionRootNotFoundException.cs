using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Occurs when composing requested but no <see cref="ICompositionRoot"/> is present in the given assembly.
    /// </summary>
    [Serializable]
    public class CompositionRootNotFoundException : Exception
    {
        /// <summary>
        /// Constructs a <see cref="CompositionRootNotFoundException"/>.
        /// </summary>
        /// <param name="assembly">The scanned assembly.</param>
        /// <param name="innerException">The inner exception.</param>
        public CompositionRootNotFoundException(Assembly assembly, Exception? innerException = null)
            : base($"No ICompositionRoot found in the given assembly: {assembly.FullName}.", innerException)
        { }

        /// <inheritdoc />
        protected CompositionRootNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
