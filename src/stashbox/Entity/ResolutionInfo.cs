using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Stashbox.Utils;

namespace Stashbox.Entity
{
    /// <summary>
    /// Represents information about the actual resolution flow.
    /// </summary>
    public class ResolutionInfo
    {
        /// <summary>
        /// Static factory for <see cref="ResolutionInfo"/>.
        /// </summary>
        /// <returns>A new <see cref="ResolutionInfo"/> instance.</returns>
        public static ResolutionInfo New() => new ResolutionInfo();

        /// <summary>
        /// The extra parameter expressions.
        /// </summary>
        public ParameterExpression[] ParameterExpressions { get; set; }

        internal HashSet<Type> CircularDependencyBarrier { get; }

        internal AvlTree<Type, Expression> ExpressionOverrides { get; }

        internal HashSet<Type> CurrentlyDecoratingTypes { get; }

        internal ResolutionInfo()
        {
            this.CircularDependencyBarrier = new HashSet<Type>();
            this.ExpressionOverrides = new AvlTree<Type, Expression>();
            this.CurrentlyDecoratingTypes = new HashSet<Type>();
        }
    }
}