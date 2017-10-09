using Stashbox.Infrastructure;
using Stashbox.Utils;
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
        /// Static factory for <see cref="ResolutionInfo"/>.
        /// </summary>
        /// <returns>A new <see cref="ResolutionInfo"/> instance.</returns>
        public static ResolutionInfo New(IResolutionScope scope, bool nullResultAllowed = false) =>
            new ResolutionInfo(scope, nullResultAllowed);

        /// <summary>
        /// True if null result is allowed, otherwise false.
        /// </summary>
        public bool NullResultAllowed { get; }

        /// <summary>
        /// The currently resolving scope.
        /// </summary>
        public ParameterExpression CurrentScopeParameter { get; }

        private AvlTree<int> circularDependencyBarrier;

        private AvlTree<Expression> expressionOverrides;

        private AvlTree<Type> currentlyDecoratingTypes;

        internal IResolutionScope ResolutionScope { get; }

        internal IResolutionScope RootScope { get; }

        internal IContainerContext ChildContext { get; }

        internal ArrayStore<ParameterExpression> ParameterExpressions { get; private set; }

        internal OrderedLinkedStore<ParameterExpression> NamedScopes { get; }

        internal ISet<string> ScopeNames { get; }
        
        internal ResolutionInfo(IResolutionScope scope, bool nullResultAllowed)
            : this(scope, AvlTree<int>.Empty, AvlTree<Expression>.Empty, AvlTree<Type>.Empty, ArrayStore<ParameterExpression>.Empty,
                  OrderedLinkedStore<ParameterExpression>.Empty, null, null, nullResultAllowed, Expression.Parameter(Constants.ResolutionScopeType))
        {
        }

        private ResolutionInfo(IResolutionScope scope, AvlTree<int> circularDependencyBarrier, AvlTree<Expression> expressionOverrides,
            AvlTree<Type> currentlyDecoratingTypes, ArrayStore<ParameterExpression> parameterExpressions, OrderedLinkedStore<ParameterExpression> namedScopes,
            ISet<string> scopeNames, IContainerContext childContext, bool nullResultAllowed, ParameterExpression currentScope)
        {
            this.circularDependencyBarrier = circularDependencyBarrier;
            this.expressionOverrides = expressionOverrides;
            this.currentlyDecoratingTypes = currentlyDecoratingTypes;
            this.NullResultAllowed = nullResultAllowed;
            this.ResolutionScope = scope;
            this.RootScope = scope.RootScope;
            this.CurrentScopeParameter = currentScope;
            this.ParameterExpressions = parameterExpressions;
            this.ChildContext = childContext;
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

        internal void AddCircularDependencyCheck(int regNumber, out bool updated)
        {
            this.circularDependencyBarrier = this.circularDependencyBarrier.AddOrUpdate(regNumber, regNumber, out updated);
        }

        internal void ClearCircularDependencyCheck(int regNumber)
        {
            this.circularDependencyBarrier = this.circularDependencyBarrier.AddOrUpdate(regNumber, 0, (oldValue, newValue) => newValue);
        }

        internal void AddParameterExpressions(params ParameterExpression[] parameterExpressions)
        {
            this.ParameterExpressions = this.ParameterExpressions.AddRange(parameterExpressions);
        }

        internal ResolutionInfo CreateNew(IContainerContext childContext = null, ParameterExpression currentScope = null) =>
            new ResolutionInfo(this.ResolutionScope, this.circularDependencyBarrier, this.expressionOverrides,
                this.currentlyDecoratingTypes, this.ParameterExpressions, this.NamedScopes, this.ScopeNames,
                childContext ?? this.ChildContext, this.NullResultAllowed, currentScope ?? this.CurrentScopeParameter);
    }
}