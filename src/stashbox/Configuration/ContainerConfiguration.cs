using Stashbox.Attributes;
using Stashbox.Entity;
using Stashbox.Registration;
using System;
using System.Collections.Generic;

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
        /// If it's set to true the container will set a unique registration name for every new registration even if the implementation type is the same.
        /// </summary>
        public bool SetUniqueRegistrationNames { get; internal set; }

        /// <summary>
        /// If it's set to true the container will track circular dependencies in the dependency graph and will throw an exception if any of it found.
        /// </summary>
        public bool CircularDependencyTrackingEnabled { get; internal set; }

        /// <summary>
        /// If it's set to true, the container will inject optional and default values for missing dependencies and primitive types.
        /// </summary>
        public bool OptionalAndDefaultValueInjectionEnabled { get; internal set; }

        /// <summary>
        /// If it's set to true the container will try to register the unknown type during the activation.
        /// </summary>
        public bool UnknownTypeResolutionEnabled { get; internal set; }

        /// <summary>
        /// If it's set to true, the container will inject members even whithout <see cref="DependencyAttribute"/>.
        /// </summary>
        public bool MemberInjectionWithoutAnnotationEnabled { get; internal set; }

        /// <summary>
        /// If it's set to true, the container will not track circular dependencies performed with <see cref="Lazy{T}"/>.
        /// </summary>
        public bool CircularDependenciesWithLazyEnabled { get; internal set; }

        /// <summary>
        /// The annotationless member injection rule.
        /// </summary>
        public Rules.AutoMemberInjectionRules MemberInjectionWithoutAnnotationRule { get; internal set; }

        /// <summary>
        /// Represents the configuration which will be invoked when an unknown type being registered.
        /// </summary>
        public Action<IFluentServiceRegistrator> UnknownTypeConfigurator { get; internal set; }

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        public Func<IEnumerable<ConstructorInformation>, IEnumerable<ConstructorInformation>> ConstructorSelectionRule { get; internal set; }

        internal ContainerConfiguration()
        {
            this.TrackTransientsForDisposalEnabled = false;
            this.CircularDependencyTrackingEnabled = false;
            this.OptionalAndDefaultValueInjectionEnabled = false;
            this.UnknownTypeResolutionEnabled = false;
            this.MemberInjectionWithoutAnnotationEnabled = false;
            this.CircularDependenciesWithLazyEnabled = false;
            this.SetUniqueRegistrationNames = false;
        }
    }
}
