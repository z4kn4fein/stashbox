using System;

namespace Stashbox.Entity
{
    /// <summary>
    /// Represents the wrapper delegate information.
    /// </summary>
    public class WrappedDelegateInformation
    {
        /// <summary>
        /// The delegate return type.
        /// </summary>
        public Type DelegateReturnType { get; set; }

        /// <summary>
        /// The wrapped type.
        /// </summary>
        public Type WrappedType { get; set; }

        /// <summary>
        /// The dependency name.
        /// </summary>
        public string DependencyName { get; set; }
    }
}
