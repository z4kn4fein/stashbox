using Stashbox.ContainerExtension;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;

namespace Stashbox
{
    /// <summary>
    /// Represents a dependency injection container.
    /// </summary>
    public interface IStashboxContainer : IDependencyRegistrator, IDependencyResolver, IDependencyReMapper, IDependencyCollectionRegistrator, IDecoratorRegistrator, IFuncRegistrator
    {
        /// <summary>
        /// Registers a <see cref="IContainerExtension"/> into the container.
        /// </summary>
        /// <param name="containerExtension">The container extension.</param>
        void RegisterExtension(IContainerExtension containerExtension);

        /// <summary>
        /// Registers an <see cref="IResolver"/> into the container.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        void RegisterResolver(IResolver resolver);

        /// <summary>
        /// Creates a child container.
        /// </summary>
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
        /// The root scope.
        /// </summary>
        IResolutionScope RootScope { get; }

        /// <summary>
        /// The service registrator.
        /// </summary>
        IServiceRegistrator ServiceRegistrator { get; }

        /// <summary>
        /// Checks a type can be resolved by the container.
        /// </summary>
        /// <typeparam name="TFrom">The service type.</typeparam>
        /// <param name="name">The registration name.</param>
        /// <returns>True if the service can be resolved, otherwise false.</returns>
        bool CanResolve<TFrom>(object name = null);

        /// <summary>
        /// Checks a type can be resolved by the container.
        /// </summary>
        /// <param name="typeFrom">The service type.</param>
        /// <param name="name">The registration name.</param>
        /// <returns>True if the service can be resolved, otherwise false.</returns>
        bool CanResolve(Type typeFrom, object name = null);

        /// <summary>
        /// Checks a type is registered in the container.
        /// </summary>
        /// <typeparam name="TFrom">The service type.</typeparam>
        /// <param name="name">The registration name.</param>
        /// <returns>True if the service is registered, otherwise false.</returns>
        bool IsRegistered<TFrom>(object name = null);

        /// <summary>
        /// Checks a type is registered in the container.
        /// </summary>
        /// <param name="typeFrom">The service type.</param>
        /// <param name="name">The registration name.</param>
        /// <returns>True if the service is registered, otherwise false.</returns>
        bool IsRegistered(Type typeFrom, object name = null);

        /// <summary>
        /// Configures the container.
        /// </summary>
        /// <param name="config">The action delegate which will configure the container.</param>
        void Configure(Action<IContainerConfigurator> config);

        /// <summary>
        /// Validates the current state of the container.
        /// </summary>
        void Validate();
    }
}
