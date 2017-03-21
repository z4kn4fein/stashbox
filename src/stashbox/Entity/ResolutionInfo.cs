using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Stashbox.Infrastructure;
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
        public static ResolutionInfo New(IResolutionScope scope, bool nullResultAllowed = false) => new ResolutionInfo(scope, nullResultAllowed);

        /// <summary>
        /// The extra parameter expressions.
        /// </summary>
        public ParameterExpression[] ParameterExpressions { get; set; }

        /// <summary>
        /// True if null result is allowed, otherwise false.
        /// </summary>
        public bool NullResultAllowed { get; }

        internal SyncTree<Type> CircularDependencyBarrier { get; }

        internal SyncTree<Type, Expression> ExpressionOverrides { get; }

        internal SyncTree<Type> CurrentlyDecoratingTypes { get; }

        internal IResolutionScope ResolutionScope { get; }

        internal ResolutionInfo(IResolutionScope scope, bool nullResultAllowed = false)
        {
            this.CircularDependencyBarrier = new SyncTree<Type>();
            this.ExpressionOverrides = new SyncTree<Type, Expression>();
            this.CurrentlyDecoratingTypes = new SyncTree<Type>();
            this.NullResultAllowed = nullResultAllowed;
            this.ResolutionScope = scope;
        }
    }
}