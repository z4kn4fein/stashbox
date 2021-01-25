using Stashbox.Lifetime;
using Stashbox.Registration.Fluent;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        internal ContainerConfigurator(ContainerConfiguration containerConfiguration = null)
        {
            this.ContainerConfiguration = containerConfiguration ?? ContainerConfiguration.DefaultContainerConfiguration();
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
        /// Enables or disables the runtime circular dependency tracking which means that the container generates checking calls into the expression tree
        /// to detect recursive references even in factory delegates passed by the user during the registration.
        /// </summary>
        /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithRuntimeCircularDependencyTracking(bool enabled = true)
        {
            this.ContainerConfiguration.RuntimeCircularDependencyTrackingEnabled = enabled;
            return this;
        }

        /// <summary>
        /// Allows circular dependencies through Lazy objects.
        /// </summary>
        /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithCircularDependencyWithLazy(bool enabled = true)
        {
            this.ContainerConfiguration.CircularDependenciesWithLazyEnabled = enabled;
            return this;
        }

        /// <summary>
        /// Enables the default value injection.
        /// </summary>
        /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithDefaultValueInjection(bool enabled = true)
        {
            this.ContainerConfiguration.DefaultValueInjectionEnabled = enabled;
            return this;
        }

        /// <summary>
        /// Enables the unknown type resolution.
        /// </summary>
        /// <param name="configurator">An optional configuration action used during the registration of the unknown type.</param>
        /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithUnknownTypeResolution(Action<UnknownRegistrationConfigurator> configurator = null, bool enabled = true)
        {
            this.ContainerConfiguration.UnknownTypeResolutionEnabled = enabled;
            this.ContainerConfiguration.UnknownTypeConfigurator = configurator;
            return this;
        }

        /// <summary>
        /// Enables the member injection without annotation.
        /// </summary>
        /// <param name="rule">The rule used to determine what kind of members (properties / fields) should be auto injected.</param>
        /// <param name="filter">An optional filter predicate used to select which properties or fields of a type should be auto injected.</param>
        /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithAutoMemberInjection(Rules.AutoMemberInjectionRules rule = Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter, Func<MemberInfo, bool> filter = null, bool enabled = true)
        {
            this.ContainerConfiguration.AutoMemberInjectionEnabled = enabled;
            this.ContainerConfiguration.AutoMemberInjectionRule = rule;
            this.ContainerConfiguration.AutoMemberInjectionFilter = filter;
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
        /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator TreatParameterAndMemberNameAsDependencyName(bool enabled = true)
        {
            this.ContainerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled = enabled;
            return this;
        }

        /// <summary>
        /// Enables the resolution of a named registration when a request ha been made without dependency name but with the same type.
        /// </summary>
        /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithNamedDependencyResolutionForUnNamedRequests(bool enabled = true)
        {
            this.ContainerConfiguration.NamedDependencyResolutionForUnNamedRequestsEnabled = enabled;
            return this;
        }

        /// <summary>
        /// Forces the usage of Microsoft expression compiler to compile expression trees.
        /// </summary>
        /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
        /// <returns>The container configurator.</returns>
        [Obsolete("Please use .WithExpressionCompiler(Rules.ExpressionCompilers.MicrosoftExpressionCompiler) instead.")]
        public ContainerConfigurator WithMicrosoftExpressionCompiler(bool enabled = true)
        {
            this.ContainerConfiguration.ExternalExpressionCompiler = enabled ? Rules.ExpressionCompilers.MicrosoftExpressionCompiler : null;
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
        /// Enables the life-span and root resolution validation on the dependency tree.
        /// </summary>
        /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithLifetimeValidation(bool enabled = true)
        {
            this.ContainerConfiguration.LifetimeValidationEnabled = enabled;
            return this;
        }

        /// <summary>
        /// Enables the rebuilding of singletons in a child-parent container case with the dependencies overridden in the child, not affecting the already built instance in the parent.
        /// </summary>
        /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithReBuildSingletonsInChildContainer(bool enabled = true)
        {
            this.ContainerConfiguration.ReBuildSingletonsInChildContainerEnabled = enabled;
            return this;
        }

        /// <summary>
        /// Enables the variance check for generic type resolutions. 
        /// The container will take variance into account during generic type resolution and will use compatible registrations.
        /// e.g. IService{in A} is selected when IService{B} is requested and B implements/extends A.
        /// </summary>
        /// <param name="enabled">True when the feature should be enabled, otherwise false.</param>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithVariantGenericResolution(bool enabled = true)
        {
            this.ContainerConfiguration.VariantGenericResolutionEnabled = enabled;
            return this;
        }

        /// <summary>
        /// Forces the usage of an external expression tree compiler.
        /// </summary>
        /// <param name="compilerDelegate">The compiler delegate used to compile expression trees.</param>
        /// <returns>The container configurator.</returns>
        public ContainerConfigurator WithExpressionCompiler(Func<LambdaExpression, Delegate> compilerDelegate)
        {
            this.ContainerConfiguration.ExternalExpressionCompiler = compilerDelegate;
            return this;
        }
    }
}
