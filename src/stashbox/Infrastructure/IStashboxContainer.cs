using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Infrastructure.Resolution;
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
        /// <param name="resolver">The resolver.</param>
        void RegisterResolver(Resolver resolver);

        /// <summary>
        /// Begins a new scope.
        /// </summary>
        IStashboxContainer BeginScope();

        /// <summary>
        /// Stores the parent container object if has any, otherwise null.
        /// </summary>
        IStashboxContainer ParentContainer { get; }

        /// <summary>
        /// Stores the container context.
        /// </summary>
        IContainerContext ContainerContext { get; }

        /// <summary>
        /// Checks a type can be resolved by the container.
        /// </summary>
        /// <typeparam name="TFrom">The service type.</typeparam>
        /// <param name="name">The registration name.</param>
        /// <returns>True if the service can be resolved, otherwise false.</returns>
        bool CanResolve<TFrom>(string name = null);

        /// <summary>
        /// Checks a type can be resolved by the container.
        /// </summary>
        /// <param name="typeFrom">The service type.</param>
        /// <param name="name">The registration name.</param>
        /// <returns>True if the service can be resolved, otherwise false.</returns>
        bool CanResolve(Type typeFrom, string name = null);

        /// <summary>
        /// Configures the container.
        /// </summary>
        /// <param name="config">The action delegate which will configure the container.</param>
        void Configure(Action<IContainerConfigurator> config);
    }
}
