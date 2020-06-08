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
        private readonly Utils.Stack<int> circularDependencyBarrier;

        internal IContainerContext RequestInitiatorContainerContext { get; }
        internal IResolutionStrategy ResolutionStrategy { get; }
        internal ExpandableArray<Expression> SingleInstructions { get; private set; }
        internal HashTree<ParameterExpression> DefinedVariables { get; private set; }
        internal ExpandableArray<IEnumerable<Pair<bool, ParameterExpression>>> ParameterExpressions { get; private set; }
        internal Type DecoratingType { get; private set; }
        internal int CurrentLifeSpan { get; private set; }
        internal string NameOfServiceLifeSpanValidatingAgainst { get; private set; }
        internal bool PerResolutionRequestCacheEnabled { get; private set; }
        internal bool UnknownTypeCheckDisabled { get; private set; }
        internal bool ShouldFallBackToRequestInitiatorContext { get; private set; }
        internal bool FactoryDelegateCacheEnabled { get; }
        internal ExpandableArray<object> ScopeNames { get; }

        /// <summary>
        /// True if null result is allowed, otherwise false.
        /// </summary>
        public bool NullResultAllowed { get; }

        /// <summary>
        /// When it's true, it indicates that the current resolution request was made from the root scope.
        /// </summary>
        public bool IsRequestedFromRoot { get; }

        /// <summary>
        /// The currently resolving scope.
        /// </summary>
        public ParameterExpression CurrentScopeParameter { get; private set; }

        /// <summary>
        /// The context of the current container instance.
        /// </summary>
        public IContainerContext CurrentContainerContext { get; private set; }

        internal ResolutionContext(IEnumerable<object> initialScopeNames,
            IContainerContext currentContainerContext,
            IResolutionStrategy resolutionStrategy,
            bool isRequestedFromRoot,
            bool nullResultAllowed = false,
            HashTree<Type, HashTree<object, Expression>> dependencyOverrides = null)
        {

            this.DefinedVariables = new HashTree<ParameterExpression>();
            this.SingleInstructions = new ExpandableArray<Expression>();
            this.expressionOverrides = dependencyOverrides ?? new HashTree<Type, HashTree<object, Expression>>();
            this.NullResultAllowed = nullResultAllowed;
            this.CurrentScopeParameter = Constants.ResolutionScopeParameter;
            this.ParameterExpressions = new ExpandableArray<IEnumerable<Pair<bool, ParameterExpression>>>();
            this.ScopeNames = ExpandableArray<object>.FromEnumerable(initialScopeNames);
            this.circularDependencyBarrier = new Utils.Stack<int>();
            this.expressionCache = new HashTree<Expression>();
            this.factoryCache = new HashTree<Func<IResolutionScope, object>>();
            this.ResolutionStrategy = resolutionStrategy;
            this.IsRequestedFromRoot = isRequestedFromRoot;
            this.CurrentContainerContext = this.RequestInitiatorContainerContext = currentContainerContext;
            this.FactoryDelegateCacheEnabled = this.PerResolutionRequestCacheEnabled = dependencyOverrides == null;
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

        internal bool WeAreInCircle(int key) =>
            this.circularDependencyBarrier.Contains(key);

        internal void PullOutCircularDependencyBarrier(int key) =>
            this.circularDependencyBarrier.Add(key);

        internal void LetDownCircularDependencyBarrier() =>
            this.circularDependencyBarrier.Pop();

        internal ResolutionContext BeginCrossContainerContext(IContainerContext currentContainerContext)
        {
            var clone = this.Clone();
            clone.CurrentContainerContext = currentContainerContext;
            clone.ShouldFallBackToRequestInitiatorContext = clone.RequestInitiatorContainerContext != currentContainerContext;
            return clone;
        }

        internal ResolutionContext BeginNewScopeContext(KeyValuePair<object, ParameterExpression> scopeParameter)
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

        internal ResolutionContext BeginUnknownTypeCheckDisabledContext()
        {
            var clone = this.Clone();
            clone.UnknownTypeCheckDisabled = true;
            return clone;
        }

        internal ResolutionContext BeginContextWithFunctionParameters(
            IEnumerable<ParameterExpression> parameterExpressions)
        {
            var clone = this.Clone();
            clone.ParameterExpressions = ExpandableArray<IEnumerable<Pair<bool, ParameterExpression>>>.FromEnumerable(this.ParameterExpressions);
            clone.ParameterExpressions.Add(parameterExpressions.Select(p => new Pair<bool, ParameterExpression>(false, p)).CastToArray());
            clone.PerResolutionRequestCacheEnabled = false;
            return clone;
        }

        internal ResolutionContext BeginDecoratingContext(Type decoratingType)
        {
            var clone = this.Clone();
            clone.DecoratingType = decoratingType;
            return clone;
        }

        internal ResolutionContext BeginLifetimeValidationContext(int lifeSpan, string currentlyLifeSpanValidatingService)
        {
            var clone = this.Clone();
            clone.CurrentLifeSpan = lifeSpan;
            clone.NameOfServiceLifeSpanValidatingAgainst = currentlyLifeSpanValidatingService;
            return clone;
        }

        private ResolutionContext Clone() => (ResolutionContext)this.MemberwiseClone();
    }
}