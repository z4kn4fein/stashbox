using Stashbox.Configuration;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;

namespace Stashbox;

/// <summary>
/// Represents a dependency injection container.
/// </summary>
public interface IStashboxContainer : IDependencyRegistrator, IDependencyResolver, IDependencyReMapper, IDependencyCollectionRegistrator, IDecoratorRegistrator, IFuncRegistrator
{
    /// <summary>
    /// The container context.
    /// </summary>
    IContainerContext ContainerContext { get; }
    
    /// <summary>
    /// Child containers created by this container.
    /// </summary>
    IEnumerable<ReadOnlyKeyValue<object, IStashboxContainer>> ChildContainers { get; }

    /// <summary>
    /// Registers an <see cref="IResolver"/>.
    /// </summary>
    /// <param name="resolver">The resolver implementation.</param>
    /// <returns>The container itself.</returns>
    IStashboxContainer RegisterResolver(IResolver resolver);

    /// <summary>
    /// Creates a child container.
    /// </summary>
    /// <param name="config">The action delegate which will configure the child container.</param>
    /// <param name="attachToParent">If true, the new child container will be attached to the lifecycle of its parent. When the parent is being disposed, the child will be disposed with it.</param>
    IStashboxContainer CreateChildContainer(Action<ContainerConfigurator>? config = null, bool attachToParent = true);
    
    /// <summary>
    /// Creates a child container.
    /// </summary>
    /// <param name="identifier">The identifier of the child container.</param>
    /// <param name="config">The action delegate which will configure the child container.</param>
    /// <param name="attachToParent">If true, the new child container will be attached to the lifecycle of its parent. When the parent is being disposed, the child will be disposed with it.</param>
    IStashboxContainer CreateChildContainer(object identifier, Action<IStashboxContainer>? config = null, bool attachToParent = true);

    /// <summary>
    /// Returns the child container identified by <paramref name="identifier"/>.
    /// </summary>
    /// <param name="identifier">The identifier of the child container.</param>
    /// <returns>The child container if it's exist, otherwise null.</returns>
    IStashboxContainer? GetChildContainer(object identifier);
    
    /// <summary>
    /// Checks whether a type is registered in the container.
    /// </summary>
    /// <typeparam name="TFrom">The service type.</typeparam>
    /// <param name="name">The registration name.</param>
    /// <returns>True if the service is registered, otherwise false.</returns>
    bool IsRegistered<TFrom>(object? name = null);

    /// <summary>
    /// Checks whether a type is registered in the container.
    /// </summary>
    /// <param name="typeFrom">The service type.</param>
    /// <param name="name">The registration name.</param>
    /// <returns>True if the service is registered, otherwise false.</returns>
    bool IsRegistered(Type typeFrom, object? name = null);

    /// <summary>
    /// Configures the container.
    /// </summary>
    /// <param name="config">The action delegate which will configure the container.</param>
    /// <returns>The container itself.</returns>
    IStashboxContainer Configure(Action<ContainerConfigurator> config);

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