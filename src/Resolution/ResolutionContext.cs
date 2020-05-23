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
        private readonly HashTree<Expression> expressionCache;
        private readonly HashTree<Func<IResolutionScope, object>> factoryCache;
        private readonly HashTree<Type, HashTree<object, Expression>> expressionOverrides;
        private readonly HashTree<Type, Type> currentlyDecoratingTypes;
        private readonly HashTree<bool> circularDependencyBarrier;

        /// <summary>
        /// True if null result is allowed, otherwise false.
        /// </summary>
        public bool NullResultAllowed { get; }

        /// <summary>
        /// The currently resolving scope.
        /// </summary>
        public ParameterExpression CurrentScopeParameter { get; private set; }

        internal IContainerContext RequestInitiatorContainerContext { get; private set; }

        internal IContainerContext CurrentContainerContext { get; private set; }

        internal ExpandableArray<Expression> SingleInstructions { get; private set; }

        internal HashTree<ParameterExpression> DefinedVariables { get; private set; }

        internal bool ExpressionCacheEnabled { get; private set; }

        internal bool FactoryDelegateCacheEnabled { get; set; }

        internal ExpandableArray<object> ScopeNames { get; }

        internal ExpandableArray<KeyValue<bool, ParameterExpression>[]> ParameterExpressions { get; }

        internal ResolutionContext(IEnumerable<object> initialScopeNames,
            IContainerContext currentContainerContext,
            bool nullResultAllowed = false,
            HashTree<Type, HashTree<object, Expression>> dependencyOverrides = null)
        {

            this.DefinedVariables = new HashTree<ParameterExpression>();
            this.SingleInstructions = new ExpandableArray<Expression>();
            this.expressionOverrides = dependencyOverrides ?? new HashTree<Type, HashTree<object, Expression>>();
            this.currentlyDecoratingTypes = new HashTree<Type, Type>();
            this.NullResultAllowed = nullResultAllowed;
            this.CurrentScopeParameter = Constants.ResolutionScopeParameter;
            this.ParameterExpressions = new ExpandableArray<KeyValue<bool, ParameterExpression>[]>();
            this.ScopeNames = ExpandableArray<object>.FromEnumerable(initialScopeNames);
            this.circularDependencyBarrier = new HashTree<bool>();
            this.expressionCache = new HashTree<Expression>();
            this.factoryCache = new HashTree<Func<IResolutionScope, object>>();
            this.FactoryDelegateCacheEnabled = dependencyOverrides == null;
            this.CurrentContainerContext = currentContainerContext;
        }

        /// <summary>
        /// Adds a custom expression to the instruction list
        /// </summary>
        /// <param name="instruction">The custom expression.</param>
        public void AddInstruction(Expression instruction) =>
            this.SingleInstructions.Add(instruction);

        /// <summary>
        /// Adds a global keyed variable to the compiled expression tree.
        /// </summary>
        /// <param name="key">The key of the variable.</param>
        /// <param name="parameter">The variable.</param>
        public void AddDefinedVariable(int key, ParameterExpression parameter) =>
            this.DefinedVariables.Add(key, parameter);

        /// <summary>
        /// Adds a global variable to the compiled expression tree.
        /// </summary>
        /// <param name="parameter">The variable.</param>
        public void AddDefinedVariable(ParameterExpression parameter) =>
            this.DefinedVariables.Add(parameter);

        /// <summary>
        /// Gets an already defined global variable.
        /// </summary>
        /// <param name="key">The key of the variable.</param>
        /// <returns>The variable.</returns>
        public ParameterExpression GetKnownVariableOrDefault(int key) =>
             this.DefinedVariables.GetOrDefault(key);

        internal void CacheExpression(int key, Expression expression) =>
            this.expressionCache.Add(key, expression);

        internal Expression GetCachedExpression(int key) =>
            this.expressionCache.GetOrDefault(key);

        internal void CacheFactory(int key, Func<IResolutionScope, object> factory) =>
            this.factoryCache.Add(key, factory);

        internal Func<IResolutionScope, object> GetCachedFactory(int key) =>
            this.factoryCache.GetOrDefault(key);

        internal bool IsCurrentlyDecorating(Type type) =>
            this.currentlyDecoratingTypes.GetOrDefault(type) != null;

        internal void AddCurrentlyDecoratingType(Type type) =>
            this.currentlyDecoratingTypes.Add(type, type);

        internal void ClearCurrentlyDecoratingType(Type type) =>
            this.currentlyDecoratingTypes.Add(type, null);

        internal Expression GetExpressionOverrideOrDefault(Type type, object name = null) =>
            this.expressionOverrides.GetOrDefault(type)?.GetOrDefault(name ?? type, false);

        internal void SetExpressionOverride(Type type, Expression expression)
        {
            var subtree = this.expressionOverrides.GetOrDefault(type);
            if (subtree != null)
            {
                subtree.Add(type, expression);
                return;
            }

            this.expressionOverrides.Add(type, new HashTree<object, Expression>(type, expression));
        }

        internal void AddParameterExpressions(IEnumerable<ParameterExpression> parameterExpressions)
        {
            this.ParameterExpressions.Add(parameterExpressions.Select(p => new KeyValue<bool, ParameterExpression>(false, p)).CastToArray());
            this.ExpressionCacheEnabled = false;
        }

        internal void SetCircularDependencyBarrier(int key, bool value) =>
            this.circularDependencyBarrier.Add(key, value);

        internal bool GetCircularDependencyBarrier(int key) =>
            this.circularDependencyBarrier.GetOrDefault(key);

        internal ResolutionContext BeginCrossContainerContext(IContainerContext requestInitiatorContext,
            IContainerContext currentContainerContext)
        {
            var clone = this.Clone();
            clone.RequestInitiatorContainerContext = requestInitiatorContext;
            clone.CurrentContainerContext = currentContainerContext;

            return clone;
        }

        internal ResolutionContext BeginNewScopeContext(KeyValue<object, ParameterExpression> scopeParameter)
        {
            this.ScopeNames.Add(scopeParameter.Key);
            var clone = this.BeginSubGraph();
            clone.CurrentScopeParameter = scopeParameter.Value;
            return clone;
        }

        internal ResolutionContext BeginSubGraph()
        {
            var clone = this.Clone();
            clone.DefinedVariables = new HashTree<ParameterExpression>();
            clone.SingleInstructions = new ExpandableArray<Expression>();

            return clone;
        }

        private ResolutionContext Clone() => (ResolutionContext)this.MemberwiseClone();
    }
}