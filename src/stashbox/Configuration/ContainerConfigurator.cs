using System;
using System.Collections.Generic;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.Configuration
{
    /// <summary>
    /// Represents a container configurator.
    /// </summary>
    public class ContainerConfigurator : IContainerConfigurator
    {
        /// <summary>
        /// The container configuration.
        /// </summary>
        public ContainerConfiguration ContainerConfiguration { get; }

        internal ContainerConfigurator()
        {
            this.ContainerConfiguration = ContainerConfiguration.DefaultContainerConfiguration();
        }

        /// <inheritdoc />
        public IContainerConfigurator WithDisposableTransientTracking()
        {
            this.ContainerConfiguration.TrackTransientsForDisposalEnabled = true;
            return this;
        }

        /// <inheritdoc />
        public IContainerConfigurator WithCircularDependencyTracking()
        {
            this.ContainerConfiguration.CircularDependencyTrackingEnabled = true;
            return this;
        }

        /// <inheritdoc />
        public IContainerConfigurator WithCircularDependencyWithLazy()
        {
            this.ContainerConfiguration.CircularDependenciesWithLazyEnabled = true;
            return this;
        }

        /// <inheritdoc />
        public IContainerConfigurator WithOptionalAndDefaultValueInjection()
        {
            this.ContainerConfiguration.OptionalAndDefaultValueInjectionEnabled = true;
            return this;
        }

        /// <inheritdoc />
        public IContainerConfigurator WithUnknownTypeResolution()
        {
            this.ContainerConfiguration.UnknownTypeResolutionEnabled = true;
            return this;
        }

        /// <inheritdoc />
        public IContainerConfigurator WithMemberInjectionWithoutAnnotation(Rules.AutoMemberInjection rule = Rules.AutoMemberInjection.PropertiesWithPublicSetter)
        {
            this.ContainerConfiguration.MemberInjectionWithoutAnnotationEnabled = true;
            this.ContainerConfiguration.MemberInjectionWithoutAnnotationRule = rule;
            return this;
        }

        /// <inheritdoc />
        public IContainerConfigurator WithConstructorSelectionRule(Func<IEnumerable<ResolutionConstructor>, ResolutionConstructor> selectionRule)
        {
            this.ContainerConfiguration.ConstructorSelectionRule = selectionRule;
            return this;
        }

        /// <inheritdoc />
        public IContainerConfigurator WithDependencySelectionRule(Func<IEnumerable<IServiceRegistration>, IServiceRegistration> selectionRule)
        {
            this.ContainerConfiguration.DependencySelectionRule = selectionRule;
            return this;
        }

        /// <inheritdoc />
        public IContainerConfigurator WithEnumerableOrderRule(Func<IEnumerable<IServiceRegistration>, IEnumerable<IServiceRegistration>> selectionRule)
        {
            this.ContainerConfiguration.EnumerableOrderRule = selectionRule;
            return this;
        }
    }
}
