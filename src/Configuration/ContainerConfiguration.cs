using Stashbox.Attributes;
using Stashbox.Lifetime;
using Stashbox.Registration.Fluent;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Stashbox.Configuration
{
    /// <summary>
    /// Represents a container configuration
    /// </summary>
    public class ContainerConfiguration
    {
        internal static ContainerConfiguration DefaultContainerConfiguration()
        {
            return new ContainerConfiguration
            {
                ConstructorSelectionRule = Rules.ConstructorSelection.PreferMostParameters
            };
        }

        /// <summary>
        /// If it's set to true the container will track transient objects for disposal.
        /// </summary>
        public bool TrackTransientsForDisposalEnabled { get; internal set; }

        /// <summary>
        /// The actual behavior used when a new service is going to be registered into the container. See the <see cref="Rules.RegistrationBehavior"/> enum for available options.
        /// </summary>
        public Rules.RegistrationBehavior RegistrationBehavior { get; internal set; }

        /// <summary>
        /// If it's set to true the container will track circular dependencies in the dependency graph and will throw an exception if any of it found.
        /// </summary>
        public bool CircularDependencyTrackingEnabled { get; internal set; }

        /// <summary>
        /// If it's set to true the container will track circular dependencies in the compiled delegates and will throw an exception if any of it found.
        /// </summary>
        public bool RuntimeCircularDependencyTrackingEnabled { get; internal set; }

        /// <summary>
        /// If it's set to true, the container will inject optional and default values for missing dependencies and primitive types.
        /// </summary>
        public bool OptionalAndDefaultValueInjectionEnabled { get; internal set; }

        /// <summary>
        /// If it's set to true the container will try to register the unknown type during the activation.
        /// </summary>
        public bool UnknownTypeResolutionEnabled { get; internal set; }

        /// <summary>
        /// If it's set to true, the container will inject members even without <see cref="DependencyAttribute"/>.
        /// </summary>
        public bool MemberInjectionWithoutAnnotationEnabled { get; internal set; }

        /// <summary>
        /// If it's set to true, the container will not track circular dependencies performed with <see cref="Lazy{T}"/>.
        /// </summary>
        public bool CircularDependenciesWithLazyEnabled { get; internal set; }

        /// <summary>
        /// If it's set to true, the container will treat the name of a constructor/method parameter or member name as a dependency name used by named resolution.
        /// </summary>
        public bool TreatingParameterAndMemberNameAsDependencyNameEnabled { get; internal set; }

        /// <summary>
        /// If it's set to true, the container will use an unnamed registration when a named one not found for a request with dependency name.
        /// </summary>
        public bool NamedDependencyResolutionForUnNamedRequestsEnabled { get; internal set; }

        /// <summary>
        /// If it's set to true, the container will use the default Microsoft expression compiler.
        /// </summary>
        public bool ForceUseMicrosoftExpressionCompiler { get; internal set; }

        /// <summary>
        /// The member injection rule.
        /// </summary>
        public Rules.AutoMemberInjectionRules MemberInjectionWithoutAnnotationRule { get; internal set; }

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        public Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>> ConstructorSelectionRule { get; internal set; }

        /// <summary>
        /// Represents the configuration which will be invoked when an unknown type being registered.
        /// </summary>
        public Action<RegistrationConfigurator> UnknownTypeConfigurator { get; internal set; }

        /// <summary>
        /// The action which will be invoked when the container configuration changes.
        /// </summary>
        public Action<ContainerConfiguration> ConfigurationChangedEvent { get; internal set; }

        /// <summary>
        /// A filter delegate used to determine which members should be auto injected and which are not.
        /// </summary>
        public Func<MemberInfo, bool> MemberInjectionFilter { get; internal set; }

        /// <summary>
        /// The default lifetime, used when a service isn't configured with a lifetime.
        /// </summary>
        public ILifetime DefaultLifetime { get; internal set; }

        internal ContainerConfiguration()
        {
            this.TrackTransientsForDisposalEnabled = false;
            this.CircularDependencyTrackingEnabled = false;
            this.OptionalAndDefaultValueInjectionEnabled = false;
            this.UnknownTypeResolutionEnabled = false;
            this.MemberInjectionWithoutAnnotationEnabled = false;
            this.CircularDependenciesWithLazyEnabled = false;
            this.TreatingParameterAndMemberNameAsDependencyNameEnabled = false;
            this.NamedDependencyResolutionForUnNamedRequestsEnabled = false;
            this.ForceUseMicrosoftExpressionCompiler = false;
        }
    }
}
