using System;
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
        public static ResolutionInfo New(IResolutionScope scope, IResolutionScope rootScope, bool nullResultAllowed = false) =>
            new ResolutionInfo(scope, rootScope, nullResultAllowed);

        /// <summary>
        /// The extra parameter expressions.
        /// </summary>
        public ParameterExpression[] ParameterExpressions { get; set; }

        /// <summary>
        /// True if null result is allowed, otherwise false.
        /// </summary>
        public bool NullResultAllowed { get; }

        private AvlTree<Type> circularDependencyBarrier;

        private AvlTree<Expression> expressionOverrides;

        private AvlTree<Type> currentlyDecoratingTypes;

        internal IResolutionScope ResolutionScope { get; }

        internal IResolutionScope RootScope { get; }

        internal ResolutionInfo(IResolutionScope scope, IResolutionScope rootScope, bool nullResultAllowed = false)
        {
            this.circularDependencyBarrier = AvlTree<Type>.Empty;
            this.expressionOverrides = AvlTree<Expression>.Empty;
            this.currentlyDecoratingTypes = AvlTree<Type>.Empty;
            this.NullResultAllowed = nullResultAllowed;
            this.ResolutionScope = scope;
            this.RootScope = rootScope;
        }

        internal bool IsCurrentlyDecorating(Type type) =>
            this.currentlyDecoratingTypes.GetOrDefault(type.GetHashCode()) != null;

        internal void AddCurrentlyDecoratingType(Type type)
        {
            this.currentlyDecoratingTypes = this.currentlyDecoratingTypes.AddOrUpdate(type.GetHashCode(), type);
        }

        internal void ClearCurrentlyDecoratingType(Type type)
        {
            this.currentlyDecoratingTypes = this.currentlyDecoratingTypes.AddOrUpdate(type.GetHashCode(), null, (oldValue, newValue) => newValue);
        }

        internal Expression GetExpressionOverrideOrDefault(Type type) =>
            this.expressionOverrides.GetOrDefault(type.GetHashCode());

        internal void SetExpressionOverride(Type type, Expression expression)
        {
            this.expressionOverrides = this.expressionOverrides.AddOrUpdate(type.GetHashCode(), expression, (oldValue, newValue) => newValue);
        }

        internal void AddCircularDependencyCheck(Type type, out bool updated)
        {
            this.circularDependencyBarrier = this.circularDependencyBarrier.AddOrUpdate(type.GetHashCode(), type, out updated);
        }

        internal void ClearCircularDependencyCheck(Type type)
        {
            this.circularDependencyBarrier = this.circularDependencyBarrier.AddOrUpdate(type.GetHashCode(), null, (oldValue, newValue) => newValue);
        }
    }
}