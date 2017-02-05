using Stashbox.Overrides;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.Entity
{
    /// <summary>
    /// Represents information about the actual resolution flow.
    /// </summary>
    public class ResolutionInfo
    {
        /// <summary>
        /// The override manager.
        /// </summary>
        public OverrideManager OverrideManager { get; set; }
        
        internal ParameterExpression[] ParameterExpressions { get; set; }

        internal ISet<Type> CircularDependencyBarrier { get; }

        internal ResolutionInfo()
        {
            this.CircularDependencyBarrier = new HashSet<Type>();
        }
    }
}