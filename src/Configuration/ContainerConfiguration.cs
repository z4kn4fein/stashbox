using Stashbox.Attributes;
using Stashbox.Lifetime;
using Stashbox.Registration.Fluent;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Configuration
{
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
        /// If it's set to true the container will track circular dependencies in the compiled delegates and will throw an exception if any of it found.
        /// </summary>
        [Obsolete("Please use parameterized factory registrations to avoid circular dependencies across delegates. `.Register<Service>(options => options.WithFactory<Dependency>(dependency => new Service(dependency)))`")]
        public bool RuntimeCircularDependencyTrackingEnabled { get; internal set; }

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
        /// A delegate to use external expression compilers.
        /// </summary>
        public Func<LambdaExpression, Delegate>? ExternalExpressionCompiler { get; internal set; }

        internal ContainerConfiguration() { }

        private ContainerConfiguration(bool trackTransientsForDisposalEnabled,
            Rules.RegistrationBehavior registrationBehavior,
            bool runtimeCircularDependencyTrackingEnabled,
            bool defaultValueInjectionEnabled,
            bool unknownTypeResolutionEnabled,
            bool autoMemberInjectionEnabled,
            bool treatingParameterAndMemberNameAsDependencyNameEnabled,
            bool namedDependencyResolutionForUnNamedRequestsEnabled,
            bool reBuildSingletonsInChildContainerEnabled,
            Rules.AutoMemberInjectionRules autoMemberInjectionRule,
            Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>> constructorSelectionRule,
            Action<UnknownRegistrationConfigurator>? unknownTypeConfigurator,
            Action<ContainerConfiguration>? configurationChangedEvent,
            Func<MemberInfo, bool>? autoMemberInjectionFilter,
            LifetimeDescriptor defaultLifetime,
            bool lifetimeValidationEnabled,
            Func<LambdaExpression, Delegate>? externalExpressionCompiler)
        {
            TrackTransientsForDisposalEnabled = trackTransientsForDisposalEnabled;
            RegistrationBehavior = registrationBehavior;
#pragma warning disable CS0618 // Type or member is obsolete
            RuntimeCircularDependencyTrackingEnabled = runtimeCircularDependencyTrackingEnabled;
#pragma warning restore CS0618 // Type or member is obsolete
            DefaultValueInjectionEnabled = defaultValueInjectionEnabled;
            UnknownTypeResolutionEnabled = unknownTypeResolutionEnabled;
            AutoMemberInjectionEnabled = autoMemberInjectionEnabled;
            TreatingParameterAndMemberNameAsDependencyNameEnabled = treatingParameterAndMemberNameAsDependencyNameEnabled;
            NamedDependencyResolutionForUnNamedRequestsEnabled = namedDependencyResolutionForUnNamedRequestsEnabled;
            ReBuildSingletonsInChildContainerEnabled = reBuildSingletonsInChildContainerEnabled;
            AutoMemberInjectionRule = autoMemberInjectionRule;
            ConstructorSelectionRule = constructorSelectionRule;
            UnknownTypeConfigurator = unknownTypeConfigurator;
            ConfigurationChangedEvent = configurationChangedEvent;
            AutoMemberInjectionFilter = autoMemberInjectionFilter;
            DefaultLifetime = defaultLifetime;
            LifetimeValidationEnabled = lifetimeValidationEnabled;
            ExternalExpressionCompiler = externalExpressionCompiler;
        }

        internal ContainerConfiguration Clone() =>
            new(this.TrackTransientsForDisposalEnabled,
                this.RegistrationBehavior,
#pragma warning disable CS0618 // Type or member is obsolete
                this.RuntimeCircularDependencyTrackingEnabled,
#pragma warning restore CS0618 // Type or member is obsolete
                this.DefaultValueInjectionEnabled,
                this.UnknownTypeResolutionEnabled,
                this.AutoMemberInjectionEnabled,
                this.TreatingParameterAndMemberNameAsDependencyNameEnabled,
                this.NamedDependencyResolutionForUnNamedRequestsEnabled,
                this.ReBuildSingletonsInChildContainerEnabled,
                this.AutoMemberInjectionRule,
                this.ConstructorSelectionRule,
                this.UnknownTypeConfigurator,
                this.ConfigurationChangedEvent,
                this.AutoMemberInjectionFilter,
                this.DefaultLifetime,
                this.LifetimeValidationEnabled,
                this.ExternalExpressionCompiler);
    }
}
