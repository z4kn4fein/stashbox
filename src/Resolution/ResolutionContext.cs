using Stashbox.Registration;
using Stashbox.Utils;
using Stashbox.Utils.Data;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Stashbox.Resolution;

/// <summary>
/// Represents information about the actual resolution flow.
/// </summary>
public class ResolutionContext
{
    internal class PerRequestConfiguration
    {
        public bool RequiresRequestContext { get; set; }
        public bool FactoryDelegateCacheEnabled { get; set; }
    }

    internal readonly Tree<Expression> ExpressionCache;
    internal readonly Utils.Data.Stack<object> ScopeNames;
    internal readonly PerRequestConfiguration RequestConfiguration;
    internal readonly IContainerContext RequestInitiatorContainerContext;
    internal readonly Utils.Data.Stack<int> CircularDependencyBarrier;
    internal readonly Tree<Func<IResolutionScope, IRequestContext, object>> FactoryCache;
    internal readonly HashTree<object, ConstantExpression>? ExpressionOverrides;
    internal readonly ExpandableArray<Expression> SingleInstructions;
    internal readonly Tree<ParameterExpression> DefinedVariables;
    internal readonly ExpandableArray<Pair<bool, ParameterExpression>[]> ParameterExpressions;
    internal readonly ExpandableArray<Type, Utils.Data.Stack<ServiceRegistration>> RemainingDecorators;
    internal readonly ExpandableArray<ServiceRegistration> CurrentDecorators;
    internal readonly int CurrentLifeSpan;
    internal readonly string? NameOfServiceLifeSpanValidatingAgainst;
    internal readonly bool PerResolutionRequestCacheEnabled;
    internal readonly bool UnknownTypeCheckDisabled;
    internal readonly bool ShouldFallBackToRequestInitiatorContext;
    internal readonly bool IsTopRequest;
    internal readonly RequestContext RequestContext;
    internal readonly bool IsValidationContext;

    /// <summary>
    /// True if null result is allowed, otherwise false.
    /// </summary>
    public readonly bool NullResultAllowed;

    /// <summary>
    /// When it's true, it indicates that the current resolution request was made from the root scope.
    /// </summary>
    public readonly bool IsRequestedFromRoot;

    /// <summary>
    /// Service resolution behavior.
    /// </summary>
    public readonly ResolutionBehavior ResolutionBehavior;

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

    private ResolutionContext(IEnumerable<object> initialScopeNames,
        IContainerContext currentContainerContext,
        bool isTopLevel,
        bool isRequestedFromRoot,
        bool nullResultAllowed,
        bool isValidationContext,
        object[]? dependencyOverrides,
        ResolutionBehavior resolutionBehavior,
        ImmutableTree<object, object>? knownInstances,
        ParameterExpression[]? initialParameters)
    {
        this.RequestConfiguration = new PerRequestConfiguration();
        this.DefinedVariables = new Tree<ParameterExpression>();
        this.SingleInstructions = new ExpandableArray<Expression>();
        this.RemainingDecorators = new ExpandableArray<Type, Utils.Data.Stack<ServiceRegistration>>();
        this.CurrentDecorators = new ExpandableArray<ServiceRegistration>();
        this.CircularDependencyBarrier = new Utils.Data.Stack<int>();
        this.ExpressionCache = new Tree<Expression>();
        this.FactoryCache = new Tree<Func<IResolutionScope, IRequestContext, object>>();
        this.NullResultAllowed = nullResultAllowed;
        this.IsRequestedFromRoot = isRequestedFromRoot;
        this.IsTopRequest = isTopLevel;
        this.ScopeNames = initialScopeNames.AsStack();
        this.CurrentScopeParameter = Constants.ResolutionScopeParameter;
        this.RequestContextParameter = Constants.RequestContextParameter;
        this.CurrentContainerContext = this.RequestInitiatorContainerContext = currentContainerContext;
        this.RequestConfiguration.FactoryDelegateCacheEnabled = this.PerResolutionRequestCacheEnabled = dependencyOverrides == null;
        this.RequestContext = dependencyOverrides != null ? RequestContext.FromOverrides(dependencyOverrides) : RequestContext.Begin();
        this.IsValidationContext = isValidationContext;

        this.ExpressionOverrides = dependencyOverrides == null && (knownInstances == null || knownInstances.IsEmpty)
            ? null
            : ProcessDependencyOverrides(dependencyOverrides, knownInstances);

        this.ResolutionBehavior = resolutionBehavior;

        this.ParameterExpressions = initialParameters != null
            ? new ExpandableArray<Pair<bool, ParameterExpression>[]>()
                {initialParameters.AsParameterPairs()}
            : new ExpandableArray<Pair<bool, ParameterExpression>[]>();
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
        HashTree<object, ConstantExpression>? expressionOverrides,
        ExpandableArray<Pair<bool, ParameterExpression>[]> parameterExpressions,
        ResolutionBehavior resolutionBehavior,
        RequestContext requestContext,
        bool nullResultAllowed,
        bool isRequestedFromRoot,
        bool isTopLevel,
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
        this.IsTopRequest = isTopLevel;
        this.ScopeNames = scopeNames;
        this.CurrentScopeParameter = currentScopeParameter;
        this.RequestContextParameter = requestContextParameter;
        this.CurrentContainerContext = currentContainerContext;
        this.RequestInitiatorContainerContext = requestInitiatorContainerContext;
        this.ExpressionOverrides = expressionOverrides;
        this.ResolutionBehavior = resolutionBehavior;
        this.ParameterExpressions = parameterExpressions;
        this.NameOfServiceLifeSpanValidatingAgainst = nameOfServiceLifeSpanValidatingAgainst;
        this.CurrentLifeSpan = currentLifeSpan;
        this.PerResolutionRequestCacheEnabled = perResolutionRequestCacheEnabled;
        this.UnknownTypeCheckDisabled = unknownTypeCheckDisabled;
        this.ShouldFallBackToRequestInitiatorContext = shouldFallBackToRequestInitiatorContext;
        this.RequestContext = requestContext;
        this.IsValidationContext = isValidationContext;
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
        bool isRequestedFromRoot,
        object[]? dependencyOverrides = null,
        ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default,
        ImmutableTree<object, object>? knownInstances = null,
        ParameterExpression[]? initialParameters = null) =>
        new(initialScopeNames,
            currentContainerContext,
            true,
            isRequestedFromRoot,
            false,
            false,
            dependencyOverrides,
            resolutionBehavior,
            knownInstances,
            initialParameters);

    internal static ResolutionContext BeginNullableTopLevelContext(
        IEnumerable<object> initialScopeNames,
        IContainerContext currentContainerContext,
        bool isRequestedFromRoot,
        object[]? dependencyOverrides = null,
        ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default,
        ImmutableTree<object, object>? knownInstances = null,
        ParameterExpression[]? initialParameters = null) =>
        new(initialScopeNames,
            currentContainerContext,
            true,
            isRequestedFromRoot,
            true,
            false,
            dependencyOverrides,
            resolutionBehavior,
            knownInstances,
            initialParameters);

    internal static ResolutionContext BeginValidationContext(IContainerContext currentContainerContext) =>
        new(TypeCache.EmptyArray<object>(), currentContainerContext, true, false, false, true, null, ResolutionBehavior.Default, null, null);

    internal ResolutionContext BeginSubDependencyContext() => !this.IsTopRequest ? this : this.Clone(isTopRequest: false);

    internal ResolutionContext BeginCrossContainerContext(IContainerContext currentContainerContext) =>
        this.Clone(currentContainerContext: currentContainerContext,
            shouldFallBackToRequestInitiatorContext: this.RequestInitiatorContainerContext != currentContainerContext);

    internal ResolutionContext BeginNewScopeContext(ReadOnlyKeyValue<object, ParameterExpression> scopeParameter)
    {
        this.ScopeNames.PushBack(scopeParameter.Key);
        return this.Clone(definedVariables: new Tree<ParameterExpression>(),
            singleInstructions: new ExpandableArray<Expression>(),
            cachedExpressions: new Tree<Expression>(),
            scopeNames: this.ScopeNames,
            currentScopeParameter: scopeParameter.Value);
    }

    internal ResolutionContext BeginSubGraph() =>
        this.Clone(definedVariables: new Tree<ParameterExpression>(),
            singleInstructions: new ExpandableArray<Expression>(),
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

    private static HashTree<object, ConstantExpression> ProcessDependencyOverrides(object[]? dependencyOverrides, ImmutableTree<object, object>? knownInstances)
    {
        var result = new HashTree<object, ConstantExpression>();

        if (knownInstances is { IsEmpty: false })
            foreach (var lateKnownInstance in knownInstances.Walk())
                result.Add(lateKnownInstance.Key, lateKnownInstance.Value.AsConstant(), false);

        if (dependencyOverrides == null) return result;

        foreach (var dependencyOverride in dependencyOverrides)
        {
            var type = dependencyOverride.GetType();
            var expression = dependencyOverride.AsConstant();

            result.Add(type, expression, false);

            foreach (var baseType in type.GetRegisterableInterfaceTypes().Concat(type.GetRegisterableBaseTypes()))
                result.Add(baseType, expression, false);
        }

        return result;
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
        string? nameOfServiceLifeSpanValidatingAgainst = null,
        int? currentLifeSpan = null,
        bool? isTopRequest = null,
        bool? perResolutionRequestCacheEnabled = null,
        bool? unknownTypeCheckDisabled = null,
        bool? shouldFallBackToRequestInitiatorContext = null) =>
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
            this.ResolutionBehavior,
            this.RequestContext,
            this.NullResultAllowed,
            this.IsRequestedFromRoot,
            isTopRequest ?? this.IsTopRequest,
            nameOfServiceLifeSpanValidatingAgainst ?? this.NameOfServiceLifeSpanValidatingAgainst,
            currentLifeSpan ?? this.CurrentLifeSpan,
            perResolutionRequestCacheEnabled ?? this.PerResolutionRequestCacheEnabled,
            unknownTypeCheckDisabled ?? this.UnknownTypeCheckDisabled,
            shouldFallBackToRequestInitiatorContext ?? this.ShouldFallBackToRequestInitiatorContext,
            this.IsValidationContext);
}