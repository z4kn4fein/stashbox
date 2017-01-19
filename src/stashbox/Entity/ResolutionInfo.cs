using Stashbox.Overrides;
using System;
using System.Collections.Generic;

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

        /// <summary>
        /// The factory parameters.
        /// </summary>
        public IEnumerable<object> FactoryParams { get; set; }

        /// <summary>
        /// The circular dependency tracker object.
        /// </summary>
        public ISet<Type> CircularDependencyBarrier { get; }

        internal ResolutionInfo()
        {
            this.CircularDependencyBarrier = new HashSet<Type>();
        }
    }
}