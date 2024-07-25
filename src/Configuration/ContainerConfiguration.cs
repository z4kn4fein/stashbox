﻿using Stashbox.Attributes;
using Stashbox.Lifetime;
using Stashbox.Registration.Fluent;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Stashbox.Utils.Data;

namespace Stashbox.Configuration;

/// <summary>
/// Represents a container configuration
/// </summary>
public class ContainerConfiguration
{
    /// <summary>
    /// If it's set to true the container will track transient objects for disposal.
    /// </summary>
    public bool TrackTransientsForDisposalEnabled { get; internal set; }

    /// <summary>
    /// The actual behavior used when a new service is going to be registered into the container. See the <see cref="Rules.RegistrationBehavior"/> enum for available options.
    /// </summary>
    public Rules.RegistrationBehavior RegistrationBehavior { get; internal set; }

    /// <summary>
    /// If it's set to true, the container will inject optional and default values for missing dependencies and primitive types.
    /// </summary>
    public bool DefaultValueInjectionEnabled { get; internal set; }

    /// <summary>
    /// If it's set to true the container will try to register the unknown type during the activation.
    /// </summary>
    public bool UnknownTypeResolutionEnabled { get; internal set; }

    /// <summary>
    /// If it's set to true, the container will inject members even without <see cref="DependencyAttribute"/>.
    /// </summary>
    public bool AutoMemberInjectionEnabled { get; internal set; }

    /// <summary>
    /// If it's set to true, the container will inject required fields and properties.
    /// </summary>
    public bool RequiredMemberInjectionEnabled { get; internal set; } = true;

    /// <summary>
    /// If it's set to true, the container will treat the name of a constructor/method parameter or member name as a dependency name used by named resolution.
    /// </summary>
    public bool TreatingParameterAndMemberNameAsDependencyNameEnabled { get; internal set; }

    /// <summary>
    /// If it's set to true, the container will use an unnamed registration when a named one not found for a request with dependency name.
    /// </summary>
    public bool NamedDependencyResolutionForUnNamedRequestsEnabled { get; internal set; }

    /// <summary>
    /// If it's set to true, in a child-parent container case singletons will be rebuilt with the dependencies overridden in the child, not affecting the already built instance in the parent.
    /// </summary>
    public bool ReBuildSingletonsInChildContainerEnabled { get; internal set; }

    /// <summary>
    /// The member injection rule.
    /// </summary>
    public Rules.AutoMemberInjectionRules AutoMemberInjectionRule { get; internal set; }

    /// <summary>
    /// The constructor selection rule.
    /// </summary>
    public Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>> ConstructorSelectionRule { get; internal set; } = Rules.ConstructorSelection.PreferMostParameters;

    /// <summary>
    /// Represents the configuration which will be invoked when an unknown type being registered.
    /// </summary>
    public Action<UnknownRegistrationConfigurator>? UnknownTypeConfigurator { get; internal set; }

    /// <summary>
    /// The action which will be invoked when the container configuration changes.
    /// </summary>
    public Action<ContainerConfiguration>? ConfigurationChangedEvent { get; internal set; }

    /// <summary>
    /// A filter delegate used to determine which members should be auto injected and which are not.
    /// </summary>
    public Func<MemberInfo, bool>? AutoMemberInjectionFilter { get; internal set; }

    /// <summary>
    /// The default lifetime, used when a service isn't configured with a lifetime.
    /// </summary>
    public LifetimeDescriptor DefaultLifetime { get; internal set; } = Lifetimes.Transient;

    /// <summary>
    /// When it's true, the container validates the lifetime configuration of the resolution
    /// graph via the <see cref="LifetimeDescriptor.LifeSpan"/> value,
    /// and checks that scoped services are not resolved from the root scope.
    /// </summary>
    public bool LifetimeValidationEnabled { get; internal set; }

    /// <summary>
    /// When it's true, the container checks for generic covariance and contravariance during the resolution of generic type collections.
    /// </summary>
    public bool VariantGenericTypesEnabled { get; internal set; }
    
    internal object? UniversalName { get; set; }
    
    internal ExpandableArray<Type>? AdditionalDependencyNameAttributeTypes { get; set; }
    
    internal ExpandableArray<Type>? AdditionalDependencyAttributeTypes { get; set; }

    /// <summary>
    /// A delegate to use external expression compilers.
    /// </summary>
    public Func<LambdaExpression, Delegate>? ExternalExpressionCompiler { get; internal set; }

    internal ContainerConfiguration() { }

    private ContainerConfiguration(bool trackTransientsForDisposalEnabled,
        Rules.RegistrationBehavior registrationBehavior,
        bool defaultValueInjectionEnabled,
        bool unknownTypeResolutionEnabled,
        bool autoMemberInjectionEnabled,
        bool treatingParameterAndMemberNameAsDependencyNameEnabled,
        bool namedDependencyResolutionForUnNamedRequestsEnabled,
        bool reBuildSingletonsInChildContainerEnabled,
        bool variantGenericTypesEnabled,
        Rules.AutoMemberInjectionRules autoMemberInjectionRule,
        Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>> constructorSelectionRule,
        Action<UnknownRegistrationConfigurator>? unknownTypeConfigurator,
        Action<ContainerConfiguration>? configurationChangedEvent,
        Func<MemberInfo, bool>? autoMemberInjectionFilter,
        LifetimeDescriptor defaultLifetime,
        bool lifetimeValidationEnabled,
        Func<LambdaExpression, Delegate>? externalExpressionCompiler,
        object? universalName,
        ExpandableArray<Type>? additionalDependencyNameAttributeTypes,
        ExpandableArray<Type>? additionalDependencyAttributeTypes)
    {
        this.TrackTransientsForDisposalEnabled = trackTransientsForDisposalEnabled;
        this.RegistrationBehavior = registrationBehavior;
        this.DefaultValueInjectionEnabled = defaultValueInjectionEnabled;
        this.UnknownTypeResolutionEnabled = unknownTypeResolutionEnabled;
        this.AutoMemberInjectionEnabled = autoMemberInjectionEnabled;
        this.TreatingParameterAndMemberNameAsDependencyNameEnabled = treatingParameterAndMemberNameAsDependencyNameEnabled;
        this.NamedDependencyResolutionForUnNamedRequestsEnabled = namedDependencyResolutionForUnNamedRequestsEnabled;
        this.ReBuildSingletonsInChildContainerEnabled = reBuildSingletonsInChildContainerEnabled;
        this.VariantGenericTypesEnabled = variantGenericTypesEnabled;
        this.AutoMemberInjectionRule = autoMemberInjectionRule;
        this.ConstructorSelectionRule = constructorSelectionRule;
        this.UnknownTypeConfigurator = unknownTypeConfigurator;
        this.ConfigurationChangedEvent = configurationChangedEvent;
        this.AutoMemberInjectionFilter = autoMemberInjectionFilter;
        this.DefaultLifetime = defaultLifetime;
        this.LifetimeValidationEnabled = lifetimeValidationEnabled;
        this.ExternalExpressionCompiler = externalExpressionCompiler;
        this.UniversalName = universalName;
        this.AdditionalDependencyNameAttributeTypes = additionalDependencyNameAttributeTypes;
        this.AdditionalDependencyAttributeTypes = additionalDependencyAttributeTypes;
    }

    internal ContainerConfiguration Clone() =>
        new(this.TrackTransientsForDisposalEnabled,
            this.RegistrationBehavior,
            this.DefaultValueInjectionEnabled,
            this.UnknownTypeResolutionEnabled,
            this.AutoMemberInjectionEnabled,
            this.TreatingParameterAndMemberNameAsDependencyNameEnabled,
            this.NamedDependencyResolutionForUnNamedRequestsEnabled,
            this.ReBuildSingletonsInChildContainerEnabled,
            this.VariantGenericTypesEnabled,
            this.AutoMemberInjectionRule,
            this.ConstructorSelectionRule,
            this.UnknownTypeConfigurator,
            this.ConfigurationChangedEvent,
            this.AutoMemberInjectionFilter,
            this.DefaultLifetime,
            this.LifetimeValidationEnabled,
            this.ExternalExpressionCompiler,
            this.UniversalName,
            this.AdditionalDependencyNameAttributeTypes,
            this.AdditionalDependencyAttributeTypes);
}