using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Collections.Generic;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure.Registration;
using static Stashbox.Configuration.Rules;

namespace Stashbox.Configuration
{
    /// <summary>
    /// Represents the configuration of the <see cref="StashboxContainer"/>.
    /// </summary>
    public class ContainerConfiguration
    {
        internal static ContainerConfiguration DefaultContainerConfiguration()
        {
            return new ContainerConfiguration()
              .WithConstructorSelectionRule(Rules.ConstructorSelection.ByPass)
              .WithDependencySelectionRule(Rules.DependencySelection.ByPass)
              .WithEnumerableOrderRule(Rules.EnumerableOrder.ByPass);
        }

        internal bool TrackTransientsForDisposalEnabled { get; private set; }

        internal bool ParentContainerResolutionEnabled { get; private set; }

        internal bool CircularDependencyTrackingEnabled { get; private set; }

        internal bool OptionalAndDefaultValueInjectionEnabled { get; private set; }

        internal bool UnknownTypeResolutionEnabled { get; private set; }

        internal bool MemberInjectionWithoutAnnotationEnabled { get; private set; }

        internal AutoMemberInjection MemberInjectionWithoutAnnotationRule { get; private set; }

        internal Func<IEnumerable<ResolutionConstructor>, ResolutionConstructor> ConstructorSelectionRule { get; private set; }

        internal Func<IEnumerable<IServiceRegistration>, IServiceRegistration> DependencySelectionRule { get; private set; }

        internal Func<IEnumerable<IServiceRegistration>, IEnumerable<IServiceRegistration>> EnumerableOrderRule { get; private set; }

        /// <summary>
        /// Enables the tracking of disposable transient objects.
        /// </summary>
        /// <returns>The container configuration.</returns>
        public ContainerConfiguration WithDisposableTransientTracking()
        {
            this.TrackTransientsForDisposalEnabled = true;
            return this;
        }

        /// <summary>
        /// Enables the resolution through the parent container.
        /// </summary>
        /// <returns>The container configuration.</returns>
        public ContainerConfiguration WithParentContainerResolution()
        {
            this.ParentContainerResolutionEnabled = true;
            return this;
        }

        /// <summary>
        /// Enables the circular dependency tracking.
        /// </summary>
        /// <returns>The container configuration.</returns>
        public ContainerConfiguration WithCircularDependencyTracking()
        {
            this.CircularDependencyTrackingEnabled = true;
            return this;
        }

        /// <summary>
        /// Enables the optional and default value injection.
        /// </summary>
        /// <returns>The container configuration.</returns>
        public ContainerConfiguration WithOptionalAndDefaultValueInjection()
        {
            this.OptionalAndDefaultValueInjectionEnabled = true;
            return this;
        }

        /// <summary>
        /// Enables the unknown type resolution.
        /// </summary>
        /// <returns>The container configuration.</returns>
        public ContainerConfiguration WithUnknownTypeResolution()
        {
            this.UnknownTypeResolutionEnabled = true;
            return this;
        }

        /// <summary>
        /// Enables the member injection without annotation.
        /// </summary>
        /// <returns>The container configuration.</returns>
        public ContainerConfiguration WithMemberInjectionWithoutAnnotation(AutoMemberInjection rule)
        {
            this.MemberInjectionWithoutAnnotationEnabled = true;
            this.MemberInjectionWithoutAnnotationRule = rule;
            return this;
        }

        /// <summary>
        /// Sets the constructor selection rule.
        /// </summary>
        /// <returns>The container configuration.</returns>
        public ContainerConfiguration WithConstructorSelectionRule(Func<IEnumerable<ResolutionConstructor>, ResolutionConstructor> selectionRule)
        {
            this.ConstructorSelectionRule = selectionRule;
            return this;
        }

        /// <summary>
        /// Sets the dependency selection rule.
        /// </summary>
        /// <returns>The container configuration.</returns>
        public ContainerConfiguration WithDependencySelectionRule(Func<IEnumerable<IServiceRegistration>, IServiceRegistration> selectionRule)
        {
            this.DependencySelectionRule = selectionRule;
            return this;
        }

        /// <summary>
        /// Sets the enumerable ordering rule.
        /// </summary>
        /// <returns>The container configuration.</returns>
        public ContainerConfiguration WithEnumerableOrderRule(Func<IEnumerable<IServiceRegistration>, IEnumerable<IServiceRegistration>> selectionRule)
        {
            this.EnumerableOrderRule = selectionRule;
            return this;
        }

        internal ContainerConfiguration()
        {
            this.TrackTransientsForDisposalEnabled = false;
            this.ParentContainerResolutionEnabled = false;
            this.CircularDependencyTrackingEnabled = false;
            this.OptionalAndDefaultValueInjectionEnabled = false;
            this.UnknownTypeResolutionEnabled = false;
            this.MemberInjectionWithoutAnnotationEnabled = false;
        }
    }
}
