using System;
using System.Collections.Generic;
using System.Reflection;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a dependency collection registrator.
    /// </summary>
    public interface IDependencyCollectionRegistrator
    {
        /// <summary>
        /// Registers types into the container mapped to an interface type.
        /// </summary>
        /// <typeparam name="TFrom">The interface type.</typeparam>
        /// <param name="types">Types to register.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterTypesAs<TFrom>(IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
             where TFrom : class;

        /// <summary>
        /// Registers types into the container mapped to an interface type.
        /// </summary>
        /// <param name="typeFrom">The interface type.</param>
        /// <param name="types">Types to register.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterTypesAs(Type typeFrom, IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null);

        /// <summary>
        /// Registers types into the container mapped to themselves.
        /// </summary>
        /// <param name="types">Types to register.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterTypesAsSelf(IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null);

        /// <summary>
        /// Registers types into the container.
        /// </summary>
        /// <param name="types">Types to register.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterTypes(IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null);

        /// <summary>
        /// Registers the publicly visible types from an assembly into the container.
        /// </summary>
        /// <param name="assembly">The assembly holding the types to register.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterAssembly(Assembly assembly, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null);

        /// <summary>
        /// Registers the publicly visible types from an assembly into the container mapped to themselves.
        /// </summary>
        /// <param name="assembly">The assembly holding the types to register.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterAssemblyAsSelf(Assembly assembly, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null);

        /// <summary>
        /// Registers the publicly visible types from an assembly collection into the container.
        /// </summary>
        /// <param name="assemblies">The assemblies holding the types to register.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterAssemblies(IEnumerable<Assembly> assemblies, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null);

        /// <summary>
        /// Registers the publicly visible types from an assembly collection into the container mapped to themselves.
        /// </summary>
        /// <param name="assemblies">The assemblies holding the types to register.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterAssembliesAsSelf(IEnumerable<Assembly> assemblies, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null);

        /// <summary>
        /// Registers the publicly visible types from an assembly which contains a given type into the container.
        /// </summary>
        /// <typeparam name="TFrom">The type the assembly contains.</typeparam>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterAssemblyContaining<TFrom>(Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
             where TFrom : class;

        /// <summary>
        /// Registers the publicly visible types from an assembly which contains a given type into the container.
        /// </summary>
        /// <param name="typeFrom">The type the assembly contains.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterAssemblyContaining(Type typeFrom, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null);

        /// <summary>
        /// Registers the publicly visible types from an assembly which contains a given type into the container mapped to themselves.
        /// </summary>
        /// <typeparam name="TFrom">The type the assembly contains.</typeparam>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterAssemblyAsSelfContaining<TFrom>(Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
             where TFrom : class;

        /// <summary>
        /// Registers the publicly visible types from an assembly which contains a given type into the container mapped to themselves.
        /// </summary>
        /// <param name="typeFrom">The type the assembly contains.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDependencyRegistrator RegisterAssemblyAsSelfContaining(Type typeFrom, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null);

        /// <summary>
        /// Composes services by calling the <see cref="ICompositionRoot.Compose"/> method of the given type parameter.
        /// </summary>
        /// <typeparam name="TCompositionRoot">The type of an <see cref="ICompositionRoot"/> implementation.</typeparam>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDecoratorRegistrator ComposeBy<TCompositionRoot>()
            where TCompositionRoot : ICompositionRoot, new();

        /// <summary>
        /// Composes services by calling the <see cref="ICompositionRoot.Compose"/> method of the given type.
        /// </summary>
        /// <param name="compositionRootType">The type of an <see cref="ICompositionRoot"/> implementation.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDecoratorRegistrator ComposeBy(Type compositionRootType);

        /// <summary>
        /// Searches the given assembly for <see cref="ICompositionRoot"/> implementations and invokes their <see cref="ICompositionRoot.Compose"/> method.
        /// </summary>
        /// <param name="assembly">The assembly to scan.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDecoratorRegistrator ComposeAssembly(Assembly assembly);

        /// <summary>
        /// Searches the given assemblies for <see cref="ICompositionRoot"/> implementations and invokes their <see cref="ICompositionRoot.Compose"/> method.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        IDecoratorRegistrator ComposeAssemblies(IEnumerable<Assembly> assemblies);
    }
}
