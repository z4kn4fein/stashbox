using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Stashbox.Infrastructure;

namespace Stashbox.Entity
{
    /// <summary>
    /// Represents information about the actual resolution flow.
    /// </summary>
    public class ResolutionInfo
    {
        /// <summary>
        /// The extra parameter expressions.
        /// </summary>
        public ParameterExpression[] ParameterExpressions { get; set; }

        /// <summary>
        /// The resolved type.
        /// </summary>
        public Type ResolvedType { get; set; }

        internal ISet<Type> CircularDependencyBarrier { get; }

        internal ResolutionInfo()
        {
            this.CircularDependencyBarrier = new HashSet<Type>();
        }
    }
}