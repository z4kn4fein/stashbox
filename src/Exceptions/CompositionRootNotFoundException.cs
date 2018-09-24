using System;
using System.Reflection;

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Threw when composing requested but no <see cref="ICompositionRoot"/> is present in the given assembly.
    /// </summary>
    public class CompositionRootNotFoundException : Exception
    {
        /// <summary>
        /// The scanned assembly.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Constructs a <see cref="CompositionRootNotFoundException"/>.
        /// </summary>
        /// <param name="assembly">The scanned assembly.</param>
        /// <param name="innerException">The inner exception.</param>
        public CompositionRootNotFoundException(Assembly assembly, Exception innerException = null)
            : base($"No ICompositionRoot found in the given assembly: {assembly.FullName}", innerException)
        {
            this.Assembly = assembly;
        }
    }
}
