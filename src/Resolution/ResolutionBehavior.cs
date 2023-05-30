namespace Stashbox.Resolution
{
    using System;

    /// <summary>
    /// Service resolution behavior.
    /// </summary>
    [Flags]
    public enum ResolutionBehavior
    {
        /// <summary>
        /// Indicates that services should be resolved from current and parent container.
        /// </summary>
        Default = Current | Parent,

        /// <summary>
        /// Indicates that services should be resolved from parent container (including indirect all ancestors) of the current contianer.
        /// </summary>
        Parent = 0,

        /// <summary>
        /// Indicates that services should be resolved from current.
        /// </summary>
        Current = 1 << 0,

        /// <summary>
        /// Indicates that services should only be resolved from child containers (including indirect all predecessors) of the current container.
        /// </summary>
        Children = 1 << 1
    }
}
