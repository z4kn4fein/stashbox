using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.Resolution
{
    /// <summary>
    /// Represents information about the actual resolution flow.
    /// </summary>
    public class ResolutionContext
    {
        /// <summary>
        /// Static factory for <see cref="ResolutionContext"/>.
        /// </summary>
        /// <returns>A new <see cref="ResolutionContext"/> instance.</returns>
        public static ResolutionContext New(IResolutionScope scope, bool nullResultAllowed = false) =>
            new ResolutionContext(scope, nullResultAllowed);

        /// <summary>
        /// True if null result is allowed, otherwise false.
        /// </summary>
        public bool NullResultAllowed { get; }

        /// <summary>
        /// The currently resolving scope.
        /// </summary>
        public ParameterExpression CurrentScopeParameter { get; }

        private AvlTree<Expression> expressionOverrides;

        private AvlTree<Type> currentlyDecoratingTypes;


        private ArrayStoreKeyed<object, ParameterExpression> knownVariables;

        internal IResolutionScope ResolutionScope { get; }

        internal IResolutionScope RootScope { get; }

        internal IContainerContext ChildContext { get; }
        internal ISet<object> ScopeNames { get; }

        internal ArrayStore<ParameterExpression> ParameterExpressions { get; private set; }

        internal ArrayStore<Expression> SingleInstructions { get; private set; }

        internal ArrayStoreKeyed<object, ParameterExpression> DefinedVariables { get; private set; }

        private ResolutionContext(IResolutionScope scope, bool nullResultAllowed)
            : this(scope, AvlTree<Expression>.Empty, AvlTree<Type>.Empty, ArrayStore<ParameterExpression>.Empty, scope.GetActiveScopeNames(), 
                  null, nullResultAllowed, Constants.ResolutionScopeParameter, ArrayStoreKeyed<object, ParameterExpression>.Empty)
        { }

        private ResolutionContext(IResolutionScope scope, AvlTree<Expression> expressionOverrides,
            AvlTree<Type> currentlyDecoratingTypes, ArrayStore<ParameterExpression> parameterExpressions, ISet<object> scopeNames,
            IContainerContext childContext, bool nullResultAllowed, ParameterExpression currentScope, ArrayStoreKeyed<object, ParameterExpression> knownVariables)
        {
            this.DefinedVariables = ArrayStoreKeyed<object, ParameterExpression>.Empty;
            this.SingleInstructions = ArrayStore<Expression>.Empty;
            this.expressionOverrides = expressionOverrides;
            this.currentlyDecoratingTypes = currentlyDecoratingTypes;
            this.NullResultAllowed = nullResultAllowed;
            this.ResolutionScope = scope;
            this.RootScope = scope.RootScope;
            this.CurrentScopeParameter = currentScope;
            this.ParameterExpressions = parameterExpressions;
            this.ChildContext = childContext;
            this.ScopeNames = scopeNames;
            this.knownVariables = knownVariables;
        }

        internal bool IsCurrentlyDecorating(Type type) =>
            this.currentlyDecoratingTypes.GetOrDefault(type.GetHashCode()) != null;

        internal void AddCurrentlyDecoratingType(Type type) =>
            this.currentlyDecoratingTypes = this.currentlyDecoratingTypes.AddOrUpdate(type.GetHashCode(), type);

        internal void ClearCurrentlyDecoratingType(Type type) =>
            this.currentlyDecoratingTypes = this.currentlyDecoratingTypes.AddOrUpdate(type.GetHashCode(), null, (oldValue, newValue) => newValue);

        internal Expression GetExpressionOverrideOrDefault(Type type) =>
            this.expressionOverrides.GetOrDefault(type.GetHashCode());

        internal void SetExpressionOverride(Type type, Expression expression) =>
            this.expressionOverrides = this.expressionOverrides.AddOrUpdate(type.GetHashCode(), expression, (oldValue, newValue) => newValue);

        internal void AddParameterExpressions(params ParameterExpression[] parameterExpressions) =>
            this.ParameterExpressions = this.ParameterExpressions.AddRange(parameterExpressions);

        internal void AddInstruction(Expression instruction) =>
            this.SingleInstructions = this.SingleInstructions.Add(instruction);

        internal void AddDefinedVariable(object key, ParameterExpression parameter) =>
            this.DefinedVariables = this.DefinedVariables.AddOrUpdate(key, parameter);

        internal void AddDefinedVariable(ParameterExpression parameter) =>
            this.DefinedVariables = this.DefinedVariables.AddOrUpdate(parameter, parameter);

        internal ParameterExpression GetKnownVariableOrDefault(object key) =>
            this.DefinedVariables.GetOrDefault(key) ?? this.knownVariables.GetOrDefault(key);

        internal ResolutionContext CreateNew(IContainerContext childContext = null, KeyValue<object, ParameterExpression> scopeParameter = null)
        {
            var scopeNames = this.ScopeNames;
            if (scopeParameter != null)
            {
                scopeNames = scopeNames ?? new HashSet<object>();
                scopeNames.Add(scopeParameter.Key);
            }

            return new ResolutionContext(this.ResolutionScope, this.expressionOverrides,
                 this.currentlyDecoratingTypes, this.ParameterExpressions, scopeNames, childContext ?? this.ChildContext,
                 this.NullResultAllowed, scopeParameter == null ? this.CurrentScopeParameter : scopeParameter.Value, this.DefinedVariables);
        }
    }
}