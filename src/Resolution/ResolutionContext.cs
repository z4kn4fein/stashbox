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
        public bool NullResultAllowed { get; private set; }

        /// <summary>
        /// The currently resolving scope.
        /// </summary>
        public ParameterExpression CurrentScopeParameter { get; private set; }

        internal IContainerContext ChildContext { get; private set; }

        internal ImmutableTree<Expression> ExpressionCache { get; private set; }

        internal ArrayList<Expression> SingleInstructions { get; private set; }

        internal ArrayList<object, ParameterExpression> DefinedVariables { get; private set; }

        private ImmutableTree<Type, Expression> expressionOverrides;
        private ImmutableTree<Type, Type> currentlyDecoratingTypes;
        private ImmutableTree<bool> circularDependencyBarrier;

        internal bool ShouldCacheFactoryDelegate { get; set; }

        internal IResolutionScope ResolutionScope { get; private set; }

        internal ArrayList<object> ScopeNames { get; private set; }

        internal ArrayList<ArrayList<bool, ParameterExpression>> ParameterExpressions { get; private set; }

        private ResolutionContext(IResolutionScope scope, bool nullResultAllowed, object[] dependencyOverrides)
        {

            this.DefinedVariables = ArrayList<object, ParameterExpression>.Empty;
            this.SingleInstructions = ArrayList<Expression>.Empty;
            this.expressionOverrides = ImmutableTree<Type, Expression>.Empty;
            this.currentlyDecoratingTypes = ImmutableTree<Type, Type>.Empty;
            this.NullResultAllowed = nullResultAllowed;
            this.ResolutionScope = scope;
            this.CurrentScopeParameter = Constants.ResolutionScopeParameter;
            this.ParameterExpressions = ArrayList<ArrayList<bool, ParameterExpression>>.Empty;
            this.ScopeNames = (ArrayList<object>)scope.GetActiveScopeNames();
            this.circularDependencyBarrier = ImmutableTree<bool>.Empty;
            this.ShouldCacheFactoryDelegate = dependencyOverrides == null;
            this.ExpressionCache = ImmutableTree<Expression>.Empty;

            this.ProcessDependencyOverrides(dependencyOverrides);
        }

        internal ResolutionContext()
        { }

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
        public void AddDefinedVariable(object key, ParameterExpression parameter) =>
            this.DefinedVariables.Add(key, parameter);

        /// <summary>
        /// Adds a global variable to the compiled expression tree.
        /// </summary>
        /// <param name="parameter">The variable.</param>
        public void AddDefinedVariable(ParameterExpression parameter) =>
            this.DefinedVariables.Add(parameter, parameter);

        /// <summary>
        /// Gets an already defined global variable.
        /// </summary>
        /// <param name="key">The key of the variable.</param>
        /// <returns>The variable.</returns>
        public ParameterExpression GetKnownVariableOrDefault(object key) =>
            this.DefinedVariables.GetOrDefault(key);

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
            this.ExpressionCache = this.ExpressionCache.AddOrUpdate(key, expression);

        internal Expression GetCachedExpression(int key) =>
            this.ExpressionCache.GetOrDefault(key);

        internal void AddParameterExpressions(Type scopeType, ParameterExpression[] parameterExpressions)
        {
            var length = parameterExpressions.Length;
            var newItems = new KeyValue<bool, ParameterExpression>[length];
            for (var i = 0; i < length; i++)
                newItems[i] = new KeyValue<bool, ParameterExpression>(false, parameterExpressions[i]);
            this.ParameterExpressions.Add(new ArrayList<bool, ParameterExpression>(newItems));
        }

        internal void SetCircularDependencyBarrier(int key, bool value) =>
            this.circularDependencyBarrier = this.circularDependencyBarrier.AddOrUpdate(key, value, true);

        internal bool GetCircularDependencyBarrier(int key) =>
            this.circularDependencyBarrier.GetOrDefault(key);

        internal ResolutionContext Clone(IContainerContext childContext, KeyValue<object, ParameterExpression> scopeParameter = null)
        {
            var scopeNames = this.ScopeNames;
            if (scopeParameter != null)
            {
                scopeNames = scopeNames ?? ArrayList<object>.Empty;
                scopeNames.Add(scopeParameter.Key);
            }

            var clone = this.Clone();
            clone.CurrentScopeParameter = scopeParameter == null ? this.CurrentScopeParameter : scopeParameter.Value;
            clone.ChildContext = childContext ?? this.ChildContext;
            clone.ScopeNames = scopeNames;

            return clone;
        }
        
        internal ResolutionContext Clone()
        {
#if IL_EMIT
            var clone = Cloner<ResolutionContext>.Clone(this);
#else
            var clone = (ResolutionContext)this.MemberwiseClone();
#endif
            clone.DefinedVariables = ArrayList<object, ParameterExpression>.Empty;
            clone.SingleInstructions = ArrayList<Expression>.Empty;
            return clone;
        }
    }
}