using Stashbox.Configuration;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;

namespace Stashbox
{
    /// <summary>
    /// Represents a dependency injection container.
    /// </summary>
    public interface IStashboxContainer : IDependencyRegistrator, IDependencyResolver, IDependencyReMapper, IDependencyCollectionRegistrator, IDecoratorRegistrator, IFuncRegistrator
    {
        /// <summary>
        /// Registers an <see cref="IResolver"/> into the container.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        void RegisterResolver(IResolver resolver);

        /// <summary>
        /// Creates a child container.
        /// </summary>
        /// <param name="config">The action delegate which will configure the child container.</param>
        IStashboxContainer CreateChildContainer(Action<ContainerConfigurator> config = null);

        /// <summary>
        /// Stores the container context.
        /// </summary>
        IContainerContext ContainerContext { get; }

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
        void Configure(Action<ContainerConfigurator> config);

        /// <summary>
        /// Validates the current state of the container.
        /// </summary>
        void Validate();

        /// <summary>
        /// Returns all registration mappings.
        /// </summary>
        /// <returns>The registration mappings.</returns>
        IEnumerable<KeyValuePair<Type, ServiceRegistration>> GetRegistrationMappings();

        /// <summary>
        /// Returns the details about the registrations.
        /// </summary>
        /// <returns>The detailed string representation of the registration.</returns>
        IEnumerable<RegistrationDiagnosticsInfo> GetRegistrationDiagnostics();
    }
}
