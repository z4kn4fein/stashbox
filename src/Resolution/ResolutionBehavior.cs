﻿using System;

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
    /// Upon enumerable resolution, when both <see cref="Current"/> and <see cref="Parent"/> behaviors are enabled, and the current container has the appropriate services, the resolution will prefer those and ignore the parent containers. When the current container doesn't have the requested services, the parent containers will serve the request.
    /// </summary>
    PreferEnumerableInCurrent = 1 << 3,
}