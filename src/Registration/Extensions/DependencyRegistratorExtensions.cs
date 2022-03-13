using System.Collections.Generic;

namespace Stashbox
{
    /// <summary>
    /// Represents the extension methods of <see cref="IDependencyRegistrator"/>.
    /// </summary>
    public static class DependencyRegistratorExtensions
    {
        /// <summary>
        /// Registers instances that are already constructed.
        /// </summary>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="instances">The collection of the constructed instances.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from the disposal tracking.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        public static IStashboxContainer RegisterInstances<TFrom>(this IDependencyRegistrator registrator,
            IEnumerable<TFrom> instances, bool withoutDisposalTracking = false)
            where TFrom : class
        {
            foreach (var instance in instances)
                registrator.RegisterInstance(instance, withoutDisposalTracking: withoutDisposalTracking);

            return (IStashboxContainer)registrator;
        }

        /// <summary>
        /// Registers instances that are already constructed.
        /// </summary>
        /// <param name="registrator">The dependency registrator.</param>
        /// <param name="instances">The collection of the constructed instances.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        public static IStashboxContainer RegisterInstances<TFrom>(this IDependencyRegistrator registrator,
            params TFrom[] instances)
            where TFrom : class => registrator.RegisterInstances(instances, false);
    }
}
