using Stashbox.Configuration;
using Stashbox.Lifetime;
using Stashbox.Registration.Fluent;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Stashbox
{
    /// <summary>
    /// Represents a container configurator.
    /// </summary>
    public interface IContainerConfigurator
    {
        /// <summary>
        /// The container configuration.
        /// </summary>
        ContainerConfiguration ContainerConfiguration { get; }

        /// <summary>
        /// Enables the tracking of disposable transient objects.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithDisposableTransientTracking();

        /// <summary>
        /// Sets the actual behavior used when a new service is going to be registered into the container. See the <see cref="Rules.RegistrationBehavior"/> enum for available options.
        /// </summary>
        /// <param name="registrationBehavior">The actual registration behavior.</param>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithRegistrationBehavior(Rules.RegistrationBehavior registrationBehavior);

        /// <summary>
        /// Enables the circular dependency tracking.
        /// </summary>
        /// <param name="runtimeTrackingEnabled">If it's true the container will track circular dependencies in the compiled delegates and will throw an exception if any of it found.</param>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithCircularDependencyTracking(bool runtimeTrackingEnabled = false);

        /// <summary>
        /// Allows circular dependencies through Lazy objects.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithCircularDependencyWithLazy();

        /// <summary>
        /// Enables the optional and default value injection.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithOptionalAndDefaultValueInjection();

        /// <summary>
        /// Enables the unknown type resolution.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithUnknownTypeResolution(Action<RegistrationConfigurator> configurator = null);

        /// <summary>
        /// Enables the member injection without annotation.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithMemberInjectionWithoutAnnotation(Rules.AutoMemberInjectionRules rule = Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter, Func<MemberInfo, bool> filter = null);

        /// <summary>
        /// Sets the constructor selection rule.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithConstructorSelectionRule(Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>> selectionRule);

        /// <summary>
        /// Sets a callback delegate to call when the container configuration changes.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator OnContainerConfigurationChanged(Action<ContainerConfiguration> configurationChanged);

        /// <summary>
        /// Enables conventional resolution, which means the container treats the constructor/method parameter or member names as dependency names used by named resolution.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator TreatParameterAndMemberNameAsDependencyName();

        /// <summary>
        /// Enables the resolution of a named registration when a request ha been made without dependency name but with the same type.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithNamedDependencyResolutionForUnNamedRequests();

        /// <summary>
        /// Forces the usage of Microsoft expression compiler to compile expression trees.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithMicrosoftExpressionCompiler();

        /// <summary>
        /// Sets the default lifetime, used when a service doesn't have a configured one.
        /// </summary>
        /// <param name="lifetime">The default lifetime.</param>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithDefaultLifetime(LifetimeDescriptor lifetime);
    }
}
