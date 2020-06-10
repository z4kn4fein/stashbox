using System;
using System.Reflection;
#if HAS_SERIALIZABLE
using System.Runtime.Serialization;
#endif

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Occurs when composing requested but no <see cref="ICompositionRoot"/> is present in the given assembly.
    /// </summary>
#if HAS_SERIALIZABLE
    [Serializable]
#endif
    public class CompositionRootNotFoundException : Exception
    {
        /// <summary>
        /// Constructs a <see cref="CompositionRootNotFoundException"/>.
        /// </summary>
        /// <param name="assembly">The scanned assembly.</param>
        /// <param name="innerException">The inner exception.</param>
        public CompositionRootNotFoundException(Assembly assembly, Exception innerException = null)
            : base($"No ICompositionRoot found in the given assembly: {assembly.FullName}.", innerException)
        { }

#if HAS_SERIALIZABLE
        /// <inheritdoc />
        protected CompositionRootNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
#endif
    }
}
