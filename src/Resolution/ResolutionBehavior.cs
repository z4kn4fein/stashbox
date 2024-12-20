using System;
using System.Collections.Generic;

namespace Stashbox.Resolution;

/// <summary>
/// Service resolution behavior.
/// </summary>
[Flags]
public enum ResolutionBehavior
{
    /// <summary>
    /// Indicates that both the current container (which initiated the resolution request) and its parents can participate in the resolution request's service selection.
    /// </summary>
    Default = Current | Parent,

    /// <summary>
    /// Indicates that parent containers (including all indirect ancestors) can participate in the resolution request's service selection.
    /// </summary>
    Parent = 1 << 0,

    /// <summary>
    /// Indicates that the current container (which initiated the resolution request) can participate in the service selection.
    /// </summary>
    Current = 1 << 1,
    
    /// <summary>
    /// Indicates that parent containers (including all indirect ancestors) can only provide dependencies for services that are already selected for resolution.
    /// </summary>
    ParentDependency = 1 << 2,
    
    /// <summary>
    /// Upon enumerable resolution, services from the current container (which initiated the resolution request) are preferred, ignoring services from parent containers.
    /// </summary>
    PreferEnumerableInCurrent = 1 << 3,
}