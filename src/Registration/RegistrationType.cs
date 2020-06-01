using System;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents the type of the registrations.
    /// </summary>
    public enum RegistrationType
    {
        /// <summary>
        /// Represents a regular registration, no extra work needed to resolve the underlying service.
        /// </summary>
        Default,

        /// <summary>
        /// Represents a registration which is resolved through a factory delegate.
        /// </summary>
        Factory,

        /// <summary>
        /// Represents a registration which produces closed generic registration during resolution.
        /// </summary>
        OpenGeneric,

        /// <summary>
        /// Represents a registration which holds an already instantiated service.
        /// </summary>
        Instance,

        /// <summary>
        /// Represents a registration which holds an already instantiated service, but further injections should be applied on it.
        /// </summary>
        WireUp,

        /// <summary>
        /// Represents a registration which is resolved to a <see cref="Func{TResult}"/> delegate.
        /// </summary>
        Func
    }
}
