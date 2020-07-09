using Stashbox.Utils;
using Stashbox.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Stashbox.Resolution
{
    /// <summary>
    /// Represents information about the actual resolution flow.
    /// </summary>
    public class ResolutionContext
    {
        private Tree<Expression> expressionCache;
        private readonly Tree<Func<IResolutionScope, object>> factoryCache;
        private readonly HashTree<object, Expression> expressionOverrides;
        private readonly Utils.Data.Stack<int> circularDependencyBarrier;

        internal IContainerContext RequestInitiatorContainerContext { get; }
        internal IResolutionStrategy ResolutionStrategy { get; }
        internal ExpandableArray<Expression> SingleInstructions { get; private set; }
        internal Tree<ParameterExpression> DefinedVariables { get; private set; }
        internal ExpandableArray<Pair<bool, ParameterExpression>[]> ParameterExpressions { get; private set; }
        internal int CurrentLifeSpan { get; private set; }
        internal string NameOfServiceLifeSpanValidatingAgainst { get; private set; }
        internal bool PerResolutionRequestCacheEnabled { get; private set; }
        internal bool UnknownTypeCheckDisabled { get; private set; }
        internal bool ShouldFallBackToRequestInitiatorContext { get; private set; }
        internal KeyValue<Type, Expression> DecoratingService { get; private set; }
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
            HashTree<object, Expression> dependencyOverrides = null,
            ParameterExpression[] initialParameters = null)
        {

            this.DefinedVariables = new Tree<ParameterExpression>();
            this.SingleInstructions = new ExpandableArray<Expression>();
            this.expressionOverrides = dependencyOverrides;
            this.NullResultAllowed = nullResultAllowed;
            this.CurrentScopeParameter = Constants.ResolutionScopeParameter;
            this.ParameterExpressions = initialParameters != null
                ? new ExpandableArray<Pair<bool, ParameterExpression>[]>()
                    {initialParameters.AsParameterPairs()}
                : new ExpandableArray<Pair<bool, ParameterExpression>[]>();
            this.ScopeNames = ExpandableArray<object>.FromEnumerable(initialScopeNames);
            this.circularDependencyBarrier = new Utils.Data.Stack<int>();
            this.expressionCache = new Tree<Expression>();
            this.factoryCache = new Tree<Func<IResolutionScope, object>>();
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
            this.DefinedVariables.Add(RuntimeHelpers.GetHashCode(parameter), parameter);

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
            this.expressionOverrides?.GetOrDefault(name ?? type, false);

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
            clone.DefinedVariables = new Tree<ParameterExpression>();
            clone.SingleInstructions = new ExpandableArray<Expression>();
            clone.expressionCache = new Tree<Expression>();
            return clone;
        }

        internal ResolutionContext BeginUnknownTypeCheckDisabledContext()
        {
            var clone = this.Clone();
            clone.UnknownTypeCheckDisabled = true;
            return clone;
        }

        internal ResolutionContext BeginContextWithFunctionParameters(ParameterExpression[] parameterExpressions)
        {
            var clone = this.Clone();
            clone.ParameterExpressions = new ExpandableArray<Pair<bool, ParameterExpression>[]>(this.ParameterExpressions.AsArray())
                {parameterExpressions.AsParameterPairs()};
            clone.PerResolutionRequestCacheEnabled = false;
            return clone;
        }

        internal ResolutionContext BeginDecoratingContext(Type decoratingType, Expression serviceExpression)
        {
            var clone = this.Clone();
            clone.DecoratingService = new KeyValue<Type, Expression>(decoratingType, serviceExpression);
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