using Stashbox.Entity;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static ResolutionContext New(IResolutionScope scope, bool nullResultAllowed = false, object[] dependencyOverrides = null) =>
            new ResolutionContext(scope, nullResultAllowed, dependencyOverrides);

        /// <summary>
        /// True if null result is allowed, otherwise false.
        /// </summary>
        public bool NullResultAllowed { get; }

        /// <summary>
        /// The currently resolving scope.
        /// </summary>
        public ParameterExpression CurrentScopeParameter { get; }

        private AvlTreeKeyValue<Type, Expression> expressionOverrides;
        private AvlTreeKeyValue<Type, Type> currentlyDecoratingTypes;
        private AvlTree<bool> circularDependencyBarrier;
        private AvlTree<Expression> expressionCache;

        private readonly ArrayStoreKeyed<object, ParameterExpression> knownVariables;

        internal bool ShouldCacheFactoryDelegate { get; }

        internal IResolutionScope ResolutionScope { get; }

        internal IContainerContext ChildContext { get; }

        internal ISet<object> ScopeNames { get; }

        internal ArrayStore<ArrayStoreKeyed<bool, ParameterExpression>> ParameterExpressions { get; private set; }

        internal ArrayStore<Expression> SingleInstructions { get; private set; }

        internal ArrayStoreKeyed<object, ParameterExpression> DefinedVariables { get; private set; }

        private ResolutionContext(IResolutionScope scope, bool nullResultAllowed, object[] dependencyOverrides)
            : this(scope, AvlTree<bool>.Empty, AvlTreeKeyValue<Type, Expression>.Empty, AvlTreeKeyValue<Type, Type>.Empty, ArrayStore<ArrayStoreKeyed<bool, ParameterExpression>>.Empty, scope.GetActiveScopeNames(),
                  null, nullResultAllowed, Constants.ResolutionScopeParameter, ArrayStoreKeyed<object, ParameterExpression>.Empty, AvlTree<Expression>.Empty, dependencyOverrides == null)
        {
            this.ProcessDependencyOverrides(dependencyOverrides);
        }

        private void ProcessDependencyOverrides(object[] dependencyOverrides)
        {
            if (dependencyOverrides == null)
                return;

            foreach (var dependencyOverride in dependencyOverrides)
            {
                var type = dependencyOverride.GetType();
                var expression = dependencyOverride.AsConstant();
                this.SetExpressionOverride(type, expression);

                foreach (var baseType in type.GetRegisterableInterfaceTypes().Concat(type.GetRegisterableBaseTypes()))
                    this.SetExpressionOverride(baseType, expression);
            }
        }

        private ResolutionContext(IResolutionScope scope, AvlTree<bool> circularDependencyBarrier, AvlTreeKeyValue<Type, Expression> expressionOverrides,
            AvlTreeKeyValue<Type, Type> currentlyDecoratingTypes, ArrayStore<ArrayStoreKeyed<bool, ParameterExpression>> parameterExpressions, ISet<object> scopeNames,
            IContainerContext childContext, bool nullResultAllowed, ParameterExpression currentScope, ArrayStoreKeyed<object, ParameterExpression> knownVariables, AvlTree<Expression> expressionCache, bool shouldCacheFactoryDelegate)
        {
            this.DefinedVariables = ArrayStoreKeyed<object, ParameterExpression>.Empty;
            this.SingleInstructions = ArrayStore<Expression>.Empty;
            this.expressionOverrides = expressionOverrides;
            this.currentlyDecoratingTypes = currentlyDecoratingTypes;
            this.NullResultAllowed = nullResultAllowed;
            this.ResolutionScope = scope;
            this.CurrentScopeParameter = currentScope;
            this.ParameterExpressions = parameterExpressions;
            this.ChildContext = childContext;
            this.ScopeNames = scopeNames;
            this.knownVariables = knownVariables;
            this.circularDependencyBarrier = circularDependencyBarrier;
            this.ShouldCacheFactoryDelegate = shouldCacheFactoryDelegate;
            this.expressionCache = expressionCache;
        }

        /// <summary>
        /// Adds a custom expression to the instruction list
        /// </summary>
        /// <param name="instruction">The custom expression.</param>
        public void AddInstruction(Expression instruction) =>
            this.SingleInstructions = this.SingleInstructions.Add(instruction);

        /// <summary>
        /// Adds a global keyed variable to the compiled expression tree.
        /// </summary>
        /// <param name="key">The key of the variable.</param>
        /// <param name="parameter">The variable.</param>
        public void AddDefinedVariable(object key, ParameterExpression parameter) =>
            this.DefinedVariables = this.DefinedVariables.AddOrUpdate(key, parameter);

        /// <summary>
        /// Adds a global variable to the compiled expression tree.
        /// </summary>
        /// <param name="parameter">The variable.</param>
        public void AddDefinedVariable(ParameterExpression parameter) =>
            this.DefinedVariables = this.DefinedVariables.AddOrUpdate(parameter, parameter);

        /// <summary>
        /// Gets an already defined global variable.
        /// </summary>
        /// <param name="key">The key of the variable.</param>
        /// <returns>The variable.</returns>
        public ParameterExpression GetKnownVariableOrDefault(object key) =>
            this.DefinedVariables.GetOrDefault(key) ?? this.knownVariables.GetOrDefault(key);

        internal bool IsCurrentlyDecorating(Type type) =>
            this.currentlyDecoratingTypes.GetOrDefault(type) != null;

        internal void AddCurrentlyDecoratingType(Type type) =>
            this.currentlyDecoratingTypes = this.currentlyDecoratingTypes.AddOrUpdate(type, type);

        internal void ClearCurrentlyDecoratingType(Type type) =>
            this.currentlyDecoratingTypes = this.currentlyDecoratingTypes.AddOrUpdate(type, null, true);

        internal Expression GetExpressionOverrideOrDefault(Type type) =>
            this.expressionOverrides.GetOrDefault(type);

        internal void SetExpressionOverride(Type type, Expression expression) =>
            this.expressionOverrides = this.expressionOverrides.AddOrUpdate(type, expression, true);

        internal void CacheExpression(int key, Expression expression) =>
            this.expressionCache = this.expressionCache.AddOrUpdate(key, expression);

        internal Expression GetCachedExpression(int key) =>
            this.expressionCache.GetOrDefault(key);

        internal void AddParameterExpressions(Type scopeType, ParameterExpression[] parameterExpressions)
        {
            var length = parameterExpressions.Length;
            var newItems = new KeyValue<bool, ParameterExpression>[length];
            for (var i = 0; i < length; i++)
                newItems[i] = new KeyValue<bool, ParameterExpression>(false, parameterExpressions[i]);
            this.ParameterExpressions = this.ParameterExpressions.Add(new ArrayStoreKeyed<bool, ParameterExpression>(newItems));
        }

        internal void SetCircularDependencyBarrier(int key, bool value) =>
            this.circularDependencyBarrier = this.circularDependencyBarrier.AddOrUpdate(key, value, true);

        internal bool GetCircularDependencyBarrier(int key) =>
            this.circularDependencyBarrier.GetOrDefault(key);

        internal ResolutionContext Clone(IContainerContext childContext = null, KeyValue<object, ParameterExpression> scopeParameter = null)
        {
            var scopeNames = this.ScopeNames;
            if (scopeParameter != null)
            {
                scopeNames = scopeNames ?? new HashSet<object>();
                scopeNames.Add(scopeParameter.Key);
            }

            return new ResolutionContext(this.ResolutionScope, this.circularDependencyBarrier, this.expressionOverrides,
                 this.currentlyDecoratingTypes, this.ParameterExpressions, scopeNames, childContext ?? this.ChildContext,
                 this.NullResultAllowed, scopeParameter == null ? this.CurrentScopeParameter : scopeParameter.Value,
                 this.DefinedVariables, this.expressionCache, this.ShouldCacheFactoryDelegate);
        }
    }
}