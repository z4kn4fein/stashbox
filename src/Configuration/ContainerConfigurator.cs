using Stashbox.Lifetime;
using Stashbox.Registration.Fluent;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Stashbox.Configuration
{
    /// <summary>
    /// Represents a container configurator.
    /// </summary>
    public class ContainerConfigurator
    {
        /// <summary>
        /// The container configuration.
        /// </summary>
        public ContainerConfiguration ContainerConfiguration { get; }

        internal ContainerConfigurator()
        {
            this.ContainerConfiguration = ContainerConfiguration.DefaultContainerConfiguration();
        }

        /// <summary>
        /// Enables the tracking of disposable transient objects.
        /// </summary>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithDisposableTransientTracking()
        {
            this.ContainerConfiguration.TrackTransientsForDisposalEnabled = true;
            return this;
        }

        /// <summary>
        /// Sets the actual behavior used when a new service is going to be registered into the container. See the <see cref="Rules.RegistrationBehavior"/> enum for available options.
        /// </summary>
        /// <param name="registrationBehavior">The actual registration behavior.</param>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithRegistrationBehavior(Rules.RegistrationBehavior registrationBehavior)
        {
            this.ContainerConfiguration.RegistrationBehavior = registrationBehavior;
            return this;
        }

        /// <summary>
        /// Enables the runtime circular dependency tracking which means that the container generates checking calls into the expression tree
        /// to detect recursive references even in factory delegates passed by the user during the registration.
        /// </summary>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithRuntimeCircularDependencyTracking()
        {
            this.ContainerConfiguration.RuntimeCircularDependencyTrackingEnabled = true;
            return this;
        }

        /// <summary>
        /// Allows circular dependencies through Lazy objects.
        /// </summary>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithCircularDependencyWithLazy()
        {
            this.ContainerConfiguration.CircularDependenciesWithLazyEnabled = true;
            return this;
        }

        /// <summary>
        /// Enables the default value injection.
        /// </summary>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithDefaultValueInjection()
        {
            this.ContainerConfiguration.DefaultValueInjectionEnabled = true;
            return this;
        }

        /// <summary>
        /// Enables the unknown type resolution.
        /// </summary>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithUnknownTypeResolution(Action<UnknownRegistrationConfigurator> configurator = null)
        {
            this.ContainerConfiguration.UnknownTypeResolutionEnabled = true;
            this.ContainerConfiguration.UnknownTypeConfigurator = configurator;
            return this;
        }

        /// <summary>
        /// Enables the member injection without annotation.
        /// </summary>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithAutoMemberInjection(Rules.AutoMemberInjectionRules rule = Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter, Func<MemberInfo, bool> filter = null)
        {
            this.ContainerConfiguration.MemberInjectionWithoutAnnotationEnabled = true;
            this.ContainerConfiguration.MemberInjectionWithoutAnnotationRule = rule;
            this.ContainerConfiguration.MemberInjectionFilter = filter;
            return this;
        }

        /// <summary>
        /// Sets the constructor selection rule.
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
        /// Enables conventional resolution, which means the container treats the constructor/method parameter or member names as dependency names used by named resolution.
        /// </summary>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator TreatParameterAndMemberNameAsDependencyName()
        {
            this.ContainerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled = true;
            return this;
        }

        /// <summary>
        /// Enables the resolution of a named registration when a request ha been made without dependency name but with the same type.
        /// </summary>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithNamedDependencyResolutionForUnNamedRequests()
        {
            this.ContainerConfiguration.NamedDependencyResolutionForUnNamedRequestsEnabled = true;
            return this;
        }

        /// <summary>
        /// Forces the usage of Microsoft expression compiler to compile expression trees.
        /// </summary>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithMicrosoftExpressionCompiler()
        {
            this.ContainerConfiguration.ForceUseMicrosoftExpressionCompiler = true;
            return this;
        }

        /// <summary>
        /// Sets the default lifetime, used when a service doesn't have a configured one.
        /// </summary>
        /// <param name="lifetime">The default lifetime.</param>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithDefaultLifetime(LifetimeDescriptor lifetime)
        {
            this.ContainerConfiguration.DefaultLifetime = lifetime;
            return this;
        }

        /// <summary>
        /// Disables the life-span and root resolution validation of the dependency graphs.
        /// </summary>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithoutLifetimeValidation()
        {
            this.ContainerConfiguration.LifetimeValidationEnabled = false;
            return this;
        }
    }
}
