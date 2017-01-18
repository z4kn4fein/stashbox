using System;
using System.Collections.Generic;
using Stashbox.Entity;
using Stashbox.Infrastructure;

namespace Stashbox.Configuration
{
    /// <summary>
    /// Represents the configuration of the <see cref="StashboxContainer"/>.
    /// </summary>
    public class ContainerConfiguration
    {
        internal static ContainerConfiguration DefaultContainerConfiguration = new ContainerConfiguration()
            .WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLessParameters)
            .WithDependencySelectionRule(Rules.DependencySelection.PreferFirstRegistered)
            .WithEnumerableOrderRule(Rules.EnumerableOrder.ByPass);

        /// <summary>
        /// If it's set to true the container will track the transient objects for disposal.
        /// </summary>
        public bool TrackTransientsForDisposal { get; private set; }

        /// <summary>
        /// If it's set to true the container will use the parent container for resolving a service which is not present in it's scope.
        /// </summary>
        public bool ParentContainerResolutionAllowed { get; private set; }

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        public Func<IEnumerable<ConstructorInformation>, ConstructorInformation> ConstructorSelectionRule { get; private set; }

        /// <summary>
        /// The dependency selection rule.
        /// </summary>
        public Func<IEnumerable<IServiceRegistration>, IServiceRegistration> DependencySelectionRule { get; private set; }

        /// <summary>
        /// The enumerable ordering rule.
        /// </summary>
        public Func<IEnumerable<IServiceRegistration>, IEnumerable<IServiceRegistration>> EnumerableOrderRule { get; private set; }

        /// <summary>
        /// Enables the tracking of disposable transient objects.
        /// </summary>
        /// <returns>The container configuration.</returns>
        public ContainerConfiguration WithDisposableTransientTracking()
        {
            this.TrackTransientsForDisposal = true;
            return this;
        }

        /// <summary>
        /// Enables the resolution through the parent container.
        /// </summary>
        /// <returns>The container configuration.</returns>
        public ContainerConfiguration WithParentContainerResolution()
        {
            this.ParentContainerResolutionAllowed = true;
            return this;
        }

        /// <summary>
        /// Sets the constructor selection rule.
        /// </summary>
        /// <returns>The container configuration.</returns>
        public ContainerConfiguration WithConstructorSelectionRule(Func<IEnumerable<ConstructorInformation>, ConstructorInformation> selectionRule)
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
    }
}
