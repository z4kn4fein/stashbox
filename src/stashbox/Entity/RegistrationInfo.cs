using System;

namespace Stashbox.Entity
{
    /// <summary>
    /// Represents type information about a registration.
    /// </summary>
    public class RegistrationInfo
    {
        /// <summary>
        /// Represents the type of the implementation.
        /// </summary>
        public Type TypeTo { get; set; }

        /// <summary>
        /// Represents the type of the service the implementation bound to.
        /// </summary>
        public Type TypeFrom { get; set; }
    }
}
