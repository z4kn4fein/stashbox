using Stashbox.Registration;
using Stashbox.Utils;
using Stashbox.Utils.Data;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Stashbox.Lifetime;

namespace Stashbox.Resolution;

/// <summary>
/// Represents information about the actual resolution flow.
/// </summary>
public class ResolutionContext
{
    internal class PerRequestConfiguration
    {
        public bool RequiresRequestContext;
        public bool FactoryDelegateCacheEnabled;
    }

    internal class AutoLifetimeTracker
    {
        public LifetimeDescriptor HighestRankingLifetime = Lifetimes.Transient;
    }
    
    private readonly bool shouldFallBackToRequestInitiatorContext;

    internal readonly Tree<Expression> ExpressionCache;
    internal readonly Utils.Data.Stack<object> ScopeNames;
    internal readonly PerRequestConfiguration RequestConfiguration;
    internal readonly Utils.Data.Stack<int> CircularDependencyBarrier;
    internal readonly Tree<Func<IResolutionScope, IRequestContext, object>> FactoryCache;
    internal readonly HashTree<Type, ExpandableArray<Override>>? ExpressionOverrides;
    internal readonly ExpandableArray<Expression> SingleInstructions;
    internal readonly Tree<ParameterExpression> DefinedVariables;
    internal readonly ExpandableArray<Pair<bool, ParameterExpression>[]> ParameterExpressions;
    internal readonly ExpandableArray<Type, Utils.Data.Stack<ServiceRegistration>> RemainingDecorators;
    internal readonly ExpandableArray<ServiceRegistration> CurrentDecorators;
    internal readonly IContainerContext RequestInitiatorContainerContext;
    internal readonly ResolutionBehavior RequestInitiatorResolutionBehavior;
    internal readonly int CurrentLifeSpan;
    internal readonly string? NameOfServiceLifeSpanValidatingAgainst;
    internal readonly bool PerResolutionRequestCacheEnabled;
    internal readonly bool UnknownTypeCheckDisabled;
    internal readonly RequestContext RequestContext;
    internal readonly bool IsValidationContext;
    internal readonly AutoLifetimeTracker? AutoLifetimeTracking;

    /// <summary>
    /// True if the resolution is currently in the parent context.
    /// </summary>
    public readonly bool IsInParentContext;
    
    /// <summary>
    /// True if null result is allowed, otherwise false.
    /// </summary>
    public readonly bool NullResultAllowed;

    /// <summary>
    /// When it's true, it indicates that the current resolution request was made from the root scope.
    /// </summary>
    public readonly bool IsRequestedFromRoot;

    /// <summary>
    /// The currently resolving scope.
    /// </summary>
    public readonly ParameterExpression RequestContextParameter;

    /// <summary>
    /// The currently resolving scope.
    /// </summary>
    public readonly ParameterExpression CurrentScopeParameter;

    /// <summary>
    /// The context of the current container instance.
    /// </summary>
    public readonly IContainerContext CurrentContainerContext;
    
    /// <summary>
    /// The resolution behavior.
    /// </summary>
    public readonly ResolutionBehavior ResolutionBehavior;

    private ResolutionContext(IEnumerable<object> initialScopeNames,
        IContainerContext currentContainerContext,
        ResolutionBehavior resolutionBehavior,
        bool isRequestedFromRoot,
        bool nullResultAllowed,
        bool isValidationContext,
        object[]? dependencyOverrides,
        ImmutableTree<Type, ImmutableBucket<Override>>? knownInstances,
        ParameterExpression[]? initialParameters)
    {
        this.RequestConfiguration = new PerRequestConfiguration();
        this.DefinedVariables = new Tree<ParameterExpression>();
        this.SingleInstructions = [];
        this.RemainingDecorators = [];
        this.CurrentDecorators = [];
        this.CircularDependencyBarrier = [];
        this.ExpressionCache = new Tree<Expression>();
        this.FactoryCache = new Tree<Func<IResolutionScope, IRequestContext, object>>();
        this.ResolutionBehavior = this.RequestInitiatorResolutionBehavior = resolutionBehavior;
        this.NullResultAllowed = nullResultAllowed;
        this.IsRequestedFromRoot = isRequestedFromRoot;
        this.ScopeNames = initialScopeNames.AsStack();
        this.CurrentScopeParameter = Constants.ResolutionScopeParameter;
        this.RequestContextParameter = Constants.RequestContextParameter;
        this.CurrentContainerContext = this.RequestInitiatorContainerContext = currentContainerContext;
        this.RequestConfiguration.FactoryDelegateCacheEnabled = this.PerResolutionRequestCacheEnabled = dependencyOverrides == null;
        this.RequestContext = dependencyOverrides != null ? RequestContext.FromOverrides(dependencyOverrides) : RequestContext.Begin();
        this.IsValidationContext = isValidationContext;
        this.AutoLifetimeTracking = null;

        this.ExpressionOverrides = dependencyOverrides == null && (knownInstances == null || knownInstances.IsEmpty)
            ? null
            : ProcessDependencyOverrides(dependencyOverrides, knownInstances);

        this.ParameterExpressions = initialParameters != null
            ? [initialParameters.AsParameterPairs()]
            : [];
    }

    private ResolutionContext(PerRequestConfiguration perRequestConfiguration,
        Tree<ParameterExpression> definedVariables,
        ExpandableArray<Expression> singleInstructions,
        ExpandableArray<Type, Utils.Data.Stack<ServiceRegistration>> remainingDecorators,
        ExpandableArray<ServiceRegistration> currentDecorators,
        Utils.Data.Stack<int> circularDependencyBarrier,
        Tree<Expression> cachedExpressions,
        Tree<Func<IResolutionScope, IRequestContext, object>> factoryCache,
        Utils.Data.Stack<object> scopeNames,
        ParameterExpression currentScopeParameter,
        ParameterExpression requestContextParameter,
        IContainerContext currentContainerContext,
        IContainerContext requestInitiatorContainerContext,
        HashTree<Type, ExpandableArray<Override>>? expressionOverrides,
        ExpandableArray<Pair<bool, ParameterExpression>[]> parameterExpressions,
        RequestContext requestContext,
        AutoLifetimeTracker? autoLifetimeTracker,
        ResolutionBehavior resolutionBehavior,
        ResolutionBehavior requestInitiatorResolutionBehavior,
        bool nullResultAllowed,
        bool isRequestedFromRoot,
        string? nameOfServiceLifeSpanValidatingAgainst,
        int currentLifeSpan,
        bool perResolutionRequestCacheEnabled,
        bool unknownTypeCheckDisabled,
        bool shouldFallBackToRequestInitiatorContext,
        bool isValidationContext)
    {
        this.RequestConfiguration = perRequestConfiguration;
        this.DefinedVariables = definedVariables;
        this.SingleInstructions = singleInstructions;
        this.RemainingDecorators = remainingDecorators;
        this.CurrentDecorators = currentDecorators;
        this.CircularDependencyBarrier = circularDependencyBarrier;
        this.ExpressionCache = cachedExpressions;
        this.FactoryCache = factoryCache;
        this.NullResultAllowed = nullResultAllowed;
        this.IsRequestedFromRoot = isRequestedFromRoot;
        this.ScopeNames = scopeNames;
        this.CurrentScopeParameter = currentScopeParameter;
        this.RequestContextParameter = requestContextParameter;
        this.CurrentContainerContext = currentContainerContext;
        this.RequestInitiatorContainerContext = requestInitiatorContainerContext;
        this.ExpressionOverrides = expressionOverrides;
        this.ParameterExpressions = parameterExpressions;
        this.NameOfServiceLifeSpanValidatingAgainst = nameOfServiceLifeSpanValidatingAgainst;
        this.CurrentLifeSpan = currentLifeSpan;
        this.PerResolutionRequestCacheEnabled = perResolutionRequestCacheEnabled;
        this.UnknownTypeCheckDisabled = unknownTypeCheckDisabled;
        this.shouldFallBackToRequestInitiatorContext = shouldFallBackToRequestInitiatorContext;
        this.RequestContext = requestContext;
        this.IsValidationContext = isValidationContext;
        this.ResolutionBehavior = resolutionBehavior;
        this.RequestInitiatorResolutionBehavior = requestInitiatorResolutionBehavior;
        this.AutoLifetimeTracking = autoLifetimeTracker;
        this.IsInParentContext = requestInitiatorContainerContext != currentContainerContext;
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

    internal void CacheExpression(int key, Expression expression) =>
        this.ExpressionCache.Add(key, expression);

    internal static ResolutionContext BeginTopLevelContext(
        IEnumerable<object> initialScopeNames,
        IContainerContext currentContainerContext,
        ResolutionBehavior resolutionBehavior,
        bool isRequestedFromRoot,
        bool nullResultAllowed,
        object[]? dependencyOverrides = null,
        ImmutableTree<Type, ImmutableBucket<Override>>? knownInstances = null,
        ParameterExpression[]? initialParameters = null) =>
        new(initialScopeNames,
            currentContainerContext,
            resolutionBehavior,
            isRequestedFromRoot,
            nullResultAllowed,
            false,
            dependencyOverrides,
            knownInstances,
            initialParameters);

    internal static ResolutionContext BeginValidationContext(IContainerContext currentContainerContext, ResolutionBehavior resolutionBehavior) =>
        new(TypeCache.EmptyArray<object>(), currentContainerContext, resolutionBehavior, false, false, true, null, null, null);

    internal ResolutionContext BeginParentContainerContext(IContainerContext currentContainerContext) =>
        this.Clone(currentContainerContext: currentContainerContext,
            shouldFallBackToRequestInitiator: this.RequestInitiatorContainerContext != currentContainerContext,
            resolutionBehavior: this.ResolutionBehavior | ResolutionBehavior.Current);
    
    internal ResolutionContext FallBackToRequestInitiatorIfNeeded() =>
        this.shouldFallBackToRequestInitiatorContext
            ? this.Clone(currentContainerContext: this.RequestInitiatorContainerContext,
                shouldFallBackToRequestInitiator: false,
                resolutionBehavior: this.RequestInitiatorResolutionBehavior)
            : this;

    internal ResolutionContext BeginNewScopeContext(ReadOnlyKeyValue<object, ParameterExpression> scopeParameter)
    {
        this.ScopeNames.PushBack(scopeParameter.Key);
        return this.Clone(definedVariables: new Tree<ParameterExpression>(),
            singleInstructions: [],
            cachedExpressions: new Tree<Expression>(),
            scopeNames: this.ScopeNames,
            currentScopeParameter: scopeParameter.Value);
    }

    internal ResolutionContext BeginSubGraph() =>
        this.Clone(definedVariables: new Tree<ParameterExpression>(),
            singleInstructions: [],
            cachedExpressions: new Tree<Expression>());

    internal ResolutionContext BeginUnknownTypeCheckDisabledContext() =>
        this.UnknownTypeCheckDisabled ? this : this.Clone(unknownTypeCheckDisabled: true);

    internal ResolutionContext BeginContextWithFunctionParameters(ParameterExpression[] parameterExpressions) =>
        this.Clone(parameterExpressions: new ExpandableArray<Pair<bool, ParameterExpression>[]>(this.ParameterExpressions)
            {parameterExpressions.AsParameterPairs()}, perResolutionRequestCacheEnabled: false);

    internal ResolutionContext BeginDecoratingContext(Type decoratingType, IEnumerable<ServiceRegistration> serviceRegistrations)
    {
        var newStack = new Utils.Data.Stack<ServiceRegistration>(serviceRegistrations);
        var current = newStack.Pop();
        var decorators = new ExpandableArray<Type, Utils.Data.Stack<ServiceRegistration>>(this.RemainingDecorators);
        decorators.AddOrUpdate(decoratingType, newStack);

        return this.Clone(currentDecorators: new ExpandableArray<ServiceRegistration>(this.CurrentDecorators) { current },
            remainingDecorators: decorators);
    }

    internal ResolutionContext BeginLifetimeValidationContext(int lifeSpan, string currentlyLifeSpanValidatingService) =>
        this.Clone(currentLifeSpan: lifeSpan, nameOfServiceLifeSpanValidatingAgainst: currentlyLifeSpanValidatingService);

    internal ResolutionContext BeginAutoLifetimeTrackingContext(AutoLifetimeTracker autoLifetimeTracker) =>
        this.Clone(autoLifetimeTracker: autoLifetimeTracker,
            definedVariables: new Tree<ParameterExpression>(),
            singleInstructions: [],
            cachedExpressions: new Tree<Expression>());
    
    private static HashTree<Type, ExpandableArray<Override>> ProcessDependencyOverrides(object[]? dependencyOverrides, ImmutableTree<Type, ImmutableBucket<Override>>? knownInstances)
    {
        var overrides = new HashTree<Type, ExpandableArray<Override>>();

        if (knownInstances is { IsEmpty: false })
            foreach (var lateKnownInstance in knownInstances.Walk())
                overrides.Add(lateKnownInstance.Key, new ExpandableArray<Override>(lateKnownInstance.Value.Repository));

        if (dependencyOverrides == null) return overrides;

        foreach (var dependencyOverride in dependencyOverrides)
        {
            if (dependencyOverride is Override @override)
            {
                var arr = overrides.GetOrDefault(@override.Type);
                if (arr != null) 
                    arr.Add(@override);
                else
                    overrides.Add(@override.Type, [@override]);
                continue;
            }
            
            var type = dependencyOverride.GetType();
            Type[] allTypes = [type, .. type.GetRegisterableInterfaceTypes().Concat(type.GetRegisterableBaseTypes())];
            foreach (var depType in allTypes)
            {
                var expOverride = Override.Of(depType, instance: dependencyOverride);
                var depOverride = overrides.GetOrDefault(depType);
                if (depOverride != null) 
                    depOverride.Add(expOverride);
                else 
                    overrides.Add(depType, new ExpandableArray<Override>(new [] {expOverride}));
            }
        }

        return overrides;
    }

    private ResolutionContext Clone(
        Tree<ParameterExpression>? definedVariables = null,
        ExpandableArray<Expression>? singleInstructions = null,
        ExpandableArray<Type, Utils.Data.Stack<ServiceRegistration>>? remainingDecorators = null,
        ExpandableArray<ServiceRegistration>? currentDecorators = null,
        Tree<Expression>? cachedExpressions = null,
        Utils.Data.Stack<object>? scopeNames = null,
        ParameterExpression? currentScopeParameter = null,
        IContainerContext? currentContainerContext = null,
        ExpandableArray<Pair<bool, ParameterExpression>[]>? parameterExpressions = null,
        ResolutionBehavior? resolutionBehavior = null,
        AutoLifetimeTracker? autoLifetimeTracker = null,
        string? nameOfServiceLifeSpanValidatingAgainst = null,
        int? currentLifeSpan = null,
        bool? perResolutionRequestCacheEnabled = null,
        bool? unknownTypeCheckDisabled = null,
        bool? shouldFallBackToRequestInitiator = null) =>
        new(this.RequestConfiguration,
            definedVariables ?? this.DefinedVariables,
            singleInstructions ?? this.SingleInstructions,
            remainingDecorators ?? this.RemainingDecorators,
            currentDecorators ?? this.CurrentDecorators,
            this.CircularDependencyBarrier,
            cachedExpressions ?? this.ExpressionCache,
            this.FactoryCache,
            scopeNames ?? this.ScopeNames,
            currentScopeParameter ?? this.CurrentScopeParameter,
            this.RequestContextParameter,
            currentContainerContext ?? this.CurrentContainerContext,
            this.RequestInitiatorContainerContext,
            this.ExpressionOverrides,
            parameterExpressions ?? this.ParameterExpressions,
            this.RequestContext,
            autoLifetimeTracker ?? this.AutoLifetimeTracking,
            resolutionBehavior ?? this.ResolutionBehavior,
            this.RequestInitiatorResolutionBehavior,
            this.NullResultAllowed,
            this.IsRequestedFromRoot,
            nameOfServiceLifeSpanValidatingAgainst ?? this.NameOfServiceLifeSpanValidatingAgainst,
            currentLifeSpan ?? this.CurrentLifeSpan,
            perResolutionRequestCacheEnabled ?? this.PerResolutionRequestCacheEnabled,
            unknownTypeCheckDisabled ?? this.UnknownTypeCheckDisabled,
            shouldFallBackToRequestInitiator ?? this.shouldFallBackToRequestInitiatorContext,
            this.IsValidationContext);
}
