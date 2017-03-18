using System;
using System.Collections.Generic;
using Stashbox.Attributes;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure.Registration;

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
                ConstructorSelectionRule = Rules.ConstructorSelection.ByPass,
                DependencySelectionRule = Rules.DependencySelection.ByPass,
                EnumerableOrderRule = Rules.EnumerableOrder.ByPass
            };
        }

        /// <summary>
        /// If it's set to true the container will track transient objects for disposal.
        /// </summary>
        public bool TrackTransientsForDisposalEnabled { get; internal set; }
        
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
        /// If it's set to true, the container will wither inject members whithout the <see cref="DependencyAttribute"/>.
        /// </summary>
        public bool MemberInjectionWithoutAnnotationEnabled { get; internal set; }

        /// <summary>
        /// If it's set to true, the container will not track circular dependencies performed with <see cref="Lazy{T}"/>.
        /// </summary>
        public bool CircularDependenciesWithLazyEnabled { get; internal set; }

        /// <summary>
        /// The annotationless member injection rule.
        /// </summary>
        public Rules.AutoMemberInjection MemberInjectionWithoutAnnotationRule { get; internal set; }

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        public Func<IEnumerable<ResolutionConstructor>, ResolutionConstructor> ConstructorSelectionRule { get; internal set; }

        /// <summary>
        /// The dependency selection rule.
        /// </summary>
        public Func<IEnumerable<IServiceRegistration>, IServiceRegistration> DependencySelectionRule { get; internal set; }

        /// <summary>
        /// The enumerable order rule.
        /// </summary>
        public Func<IEnumerable<IServiceRegistration>, IEnumerable<IServiceRegistration>> EnumerableOrderRule { get; internal set; }

        internal ContainerConfiguration()
        {
            this.TrackTransientsForDisposalEnabled = false;
            this.CircularDependencyTrackingEnabled = false;
            this.OptionalAndDefaultValueInjectionEnabled = false;
            this.UnknownTypeResolutionEnabled = false;
            this.MemberInjectionWithoutAnnotationEnabled = false;
            this.CircularDependenciesWithLazyEnabled = false;
        }
    }
}
