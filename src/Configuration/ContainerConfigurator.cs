using Stashbox.Lifetime;
using Stashbox.Registration.Fluent;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Stashbox.Attributes;
using Stashbox.Utils.Data;

namespace Stashbox.Configuration;

/// <summary>
/// Represents a container configurator.
/// </summary>
public class ContainerConfigurator
{
    /// <summary>
    /// The container configuration.
    /// </summary>
    public ContainerConfiguration ContainerConfiguration { get; }

    internal ContainerConfigurator(ContainerConfiguration? containerConfiguration = null)
    {
        this.ContainerConfiguration = containerConfiguration ?? new ContainerConfiguration();
    }

    /// <summary>
    /// Enables or disables the tracking of disposable transient objects.
    /// </summary>
    /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator WithDisposableTransientTracking(bool enabled = true)
    {
        this.ContainerConfiguration.TrackTransientsForDisposalEnabled = enabled;
        return this;
    }

    /// <summary>
    /// Sets the actual behavior used when a new service is registered into the container. These options do not affect named registrations. See the <see cref="Rules.RegistrationBehavior"/> enum for available options.
    /// </summary>
    /// <param name="registrationBehavior">The actual registration behavior.</param>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator WithRegistrationBehavior(Rules.RegistrationBehavior registrationBehavior)
    {
        this.ContainerConfiguration.RegistrationBehavior = registrationBehavior;
        return this;
    }

    /// <summary>
    /// Enables or disables the default value injection.
    /// </summary>
    /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator WithDefaultValueInjection(bool enabled = true)
    {
        this.ContainerConfiguration.DefaultValueInjectionEnabled = enabled;
        return this;
    }

    /// <summary>
    /// Enables or disables the unknown type resolution.
    /// </summary>
    /// <param name="configurator">An optional configuration action used during the registration of the unknown type.</param>
    /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator WithUnknownTypeResolution(Action<UnknownRegistrationConfigurator>? configurator = null, bool enabled = true)
    {
        this.ContainerConfiguration.UnknownTypeResolutionEnabled = enabled;
        this.ContainerConfiguration.UnknownTypeConfigurator = configurator;
        return this;
    }

    /// <summary>
    /// Enables or disables the auto member-injection without annotation.
    /// </summary>
    /// <param name="rule">The rule used to determine what kind of members (properties / fields) should be auto injected.</param>
    /// <param name="filter">An optional filter predicate used to select which properties or fields of a type should be auto injected.</param>
    /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator WithAutoMemberInjection(Rules.AutoMemberInjectionRules rule = Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter, Func<MemberInfo, bool>? filter = null, bool enabled = true)
    {
        this.ContainerConfiguration.AutoMemberInjectionEnabled = enabled;
        this.ContainerConfiguration.AutoMemberInjectionRule = rule;
        this.ContainerConfiguration.AutoMemberInjectionFilter = filter;
        return this;
    }
    
    /// <summary>
    /// Enables or disables required member injection.
    /// </summary>
    /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator WithRequiredMemberInjection(bool enabled = true)
    {
        this.ContainerConfiguration.RequiredMemberInjectionEnabled = enabled;
        return this;
    }

    /// <summary>
    /// Sets the constructor selection rule used to determine which constructor should the container use for instantiation
    /// </summary>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator WithConstructorSelectionRule(Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>> selectionRule)
    {
        this.ContainerConfiguration.ConstructorSelectionRule = selectionRule;
        return this;
    }

    /// <summary>
    /// Sets a callback delegate to call when the container configuration changes.
    /// </summary>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator OnContainerConfigurationChanged(Action<ContainerConfiguration> configurationChanged)
    {
        this.ContainerConfiguration.ConfigurationChangedEvent = configurationChanged;
        return this;
    }

    /// <summary>
    /// Enables or disables conventional resolution, which means the container treats the constructor/method parameter or member names as dependency names used by named resolution.
    /// </summary>
    /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator TreatParameterAndMemberNameAsDependencyName(bool enabled = true)
    {
        this.ContainerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled = enabled;
        return this;
    }

    /// <summary>
    /// Enables or disables the selection of named registrations when the resolution request is un-named but with the same type.
    /// </summary>
    /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator WithNamedDependencyResolutionForUnNamedRequests(bool enabled = true)
    {
        this.ContainerConfiguration.NamedDependencyResolutionForUnNamedRequestsEnabled = enabled;
        return this;
    }

    /// <summary>
    /// Sets the default lifetime used when a service doesn't have a configured one.
    /// </summary>
    /// <param name="lifetime">The default lifetime.</param>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator WithDefaultLifetime(LifetimeDescriptor lifetime)
    {
        this.ContainerConfiguration.DefaultLifetime = lifetime;
        return this;
    }

    /// <summary>
    /// Enables or disables the life-span and root resolution validation on the dependency tree.
    /// </summary>
    /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator WithLifetimeValidation(bool enabled = true)
    {
        this.ContainerConfiguration.LifetimeValidationEnabled = enabled;
        return this;
    }

    /// <summary>
    /// Enables or disables the re-building of singletons in child containers. It allows the child containers to effectively override singleton dependencies in the parent. This feature is not affecting the already built singleton instances in the parent.
    /// </summary>
    /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator WithReBuildSingletonsInChildContainer(bool enabled = true)
    {
        this.ContainerConfiguration.ReBuildSingletonsInChildContainerEnabled = enabled;
        return this;
    }

    /// <summary>
    /// Sets an external expression tree compiler used by the container to compile the generated expressions.
    /// </summary>
    /// <param name="compilerDelegate">The compiler delegate used to compile expression trees.</param>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator WithExpressionCompiler(Func<LambdaExpression, Delegate> compilerDelegate)
    {
        this.ContainerConfiguration.ExternalExpressionCompiler = compilerDelegate;
        return this;
    }
    
    /// <summary>
    /// Sets the universal name that represents a special name which allows named resolution work for any given name.
    /// </summary>
    /// <param name="name">The universal name.</param>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator WithUniversalName(object name)
    {
        this.ContainerConfiguration.UniversalName = name;
        return this;
    }
    
    /// <summary>
    /// Adds an attribute type that is considered a dependency name indicator just like <see cref="DependencyNameAttribute"/>. 
    /// </summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator WithAdditionalDependencyNameAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        this.ContainerConfiguration.AdditionalDependencyNameAttributeTypes ??= [];
        this.ContainerConfiguration.AdditionalDependencyNameAttributeTypes.Add(typeof(TAttribute));
        return this;
    }
    
    /// <summary>
    /// Adds an attribute type that is considered a dependency indicator just like <see cref="DependencyAttribute"/>. 
    /// </summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <returns>The container configurator.</returns>
    public ContainerConfigurator WithAdditionalDependencyAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        this.ContainerConfiguration.AdditionalDependencyAttributeTypes ??= [];
        this.ContainerConfiguration.AdditionalDependencyAttributeTypes.Add(typeof(TAttribute));
        return this;
    }
}