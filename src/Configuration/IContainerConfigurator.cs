using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Registration.Fluent;
using System;
using System.Collections.Generic;

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
        /// Enables the unique registration id generation for services even if the implementation type is the same.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithUniqueRegistrationIdentifiers();

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
        IContainerConfigurator WithMemberInjectionWithoutAnnotation(Rules.AutoMemberInjectionRules rule = Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter, Func<TypeInformation, bool> filter = null);

        /// <summary>
        /// Sets the constructor selection rule.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithConstructorSelectionRule(Func<IEnumerable<ConstructorInformation>, IEnumerable<ConstructorInformation>> selectionRule);

        /// <summary>
        /// Sets a callback delegate to call when the container configuration changes.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator OnContainerConfigurationChanged(Action<ContainerConfiguration> configurationChanged);

        /// <summary>
        /// Enables the treating of constructor/method parameter or member names as dependency names used by named resolution.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator TreatParameterOrMemberNamesAsDependencyName();

        /// <summary>
        /// Enables the resolution of an unnamed registration when a named one not found for a request with dependency name.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithUnNamedDependencyResolutionWhenNamedIsNotAvailable();
    }
}
