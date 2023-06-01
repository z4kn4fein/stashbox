﻿using System;
using System.Collections.Generic;

namespace Stashbox.Resolution;

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
    /// Indicates that services should be resolved from parent container (including indirect all ancestors) of the current container.
    /// </summary>
    Parent = 1 << 0,

    /// <summary>
    /// Indicates that services should be resolved from current.
    /// </summary>
    Current = 1 << 1,
    
    /// <summary>
    /// Indicates that services should be collected from each child container. This flag only applies to <see cref="IEnumerable{T}"/> resolution requests
    /// to include services from child containers into the result. It does not have effect on single service resolution.
    /// </summary>
    Children = 1 << 2,
}