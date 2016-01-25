using Stashbox.Entity;
using Stashbox.Infrastructure.ContainerExtension;
using System;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a dependency injection container.
    /// </summary>
    public interface IStashboxContainer : IDependencyRegistrator, IDependencyResolver, IDisposable
    {
        /// <summary>
        /// Registers a <see cref="IContainerExtension"/> into the container.
        /// </summary>
        /// <param name="containerExtension">The container extension.</param>
        void RegisterExtension(IContainerExtension containerExtension);

        /// <summary>
        /// Registers a <see cref="Resolver"/> into the container.
        /// </summary>
        /// <param name="resolverPredicate">Predicate which decides that the resolver is can be used for an actual resolution.</param>
        /// <param name="factory">The factory which produces a new instance of the resolver.</param>
        void RegisterResolver(Func<IContainerContext, TypeInformation, bool> resolverPredicate,
            Func<IContainerContext, TypeInformation, Resolver> factory);

        /// <summary>
        /// Creates a child container.
        /// </summary>
        /// <returns>The child container.</returns>
        IStashboxContainer CreateChildContainer();

        /// <summary>
        /// Stores the parent container object if has any, otherwise null.
        /// </summary>
        IStashboxContainer ParentContainer { get; }

        /// <summary>
        /// Stores the container context.
        /// </summary>
        IContainerContext ContainerContext { get; }

        /// <summary>
        /// Checks a service is registered into the container.
        /// </summary>
        /// <typeparam name="TFrom">The service type.</typeparam>
        /// <param name="name">The registration name.</param>
        /// <returns>True if the service is registered, otherwise false.</returns>
        bool IsRegistered<TFrom>(string name = null);

        /// <summary>
        /// Checks a service is registered into the container.
        /// </summary>
        /// <param name="typeFrom">The service type.</param>
        /// <param name="name">The registration name.</param>
        /// <returns>True if the service is registered, otherwise false.</returns>
        bool IsRegistered(Type typeFrom, string name = null);
    }
}
