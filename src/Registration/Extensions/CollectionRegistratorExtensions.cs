﻿using Stashbox.Exceptions;
using Stashbox.Registration.Fluent;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox
{
    /// <summary>
    /// Represents the extension methods of <see cref="IDependencyCollectionRegistrator"/>.
    /// </summary>
    public static class CollectionRegistratorExtensions
    {
        /// <summary>
        /// Registers types into the container mapped to an interface type.
        /// </summary>
        /// <typeparam name="TFrom">The interface type.</typeparam>
        /// <param name="registrator">The registrator.</param>
        /// <param name="types">Types to register.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer RegisterTypesAs<TFrom>(this IDependencyCollectionRegistrator registrator, 
            IEnumerable<Type> types, 
            Func<Type, bool> selector = null, 
            Action<RegistrationConfigurator> configurator = null)
            where TFrom : class =>
            registrator.RegisterTypesAs(typeof(TFrom), 
                types, 
                selector, 
                configurator);

        /// <summary>
        /// Registers types into the container mapped to an interface type.
        /// </summary>
        /// <typeparam name="TFrom">The interface type.</typeparam>
        /// <param name="registrator">The registrator.</param>
        /// <param name="assembly">Assembly to register.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer RegisterTypesAs<TFrom>(this IDependencyCollectionRegistrator registrator, 
            Assembly assembly, 
            Func<Type, bool> selector = null, 
            Action<RegistrationConfigurator> configurator = null)
            where TFrom : class =>
            registrator.RegisterTypesAs(typeof(TFrom), 
                assembly.CollectTypes(), 
                selector, 
                configurator);

        /// <summary>
        /// Registers types into the container mapped to an interface type.
        /// </summary>
        /// <param name="typeFrom">The interface type.</param>
        /// <param name="registrator">The registrator.</param>
        /// <param name="assembly">Assembly to register.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer RegisterTypesAs(this IDependencyCollectionRegistrator registrator, 
            Type typeFrom, 
            Assembly assembly, 
            Func<Type, bool> selector = null, 
            Action<RegistrationConfigurator> configurator = null) =>
            registrator.RegisterTypesAs(typeFrom, 
                assembly.CollectTypes(), 
                selector, 
                configurator);

        /// <summary>
        /// Registers the publicly visible types from an assembly into the container.
        /// </summary>
        /// <param name="registrator">The registrator.</param>
        /// <param name="assembly">The assembly holding the types to register.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="serviceTypeSelector">The service type selector. Used to filter the service types the actual type bound to.</param>
        /// <param name="registerSelf">If it's true the types will be registered to their own type too.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer RegisterAssembly(this IDependencyCollectionRegistrator registrator, 
            Assembly assembly, 
            Func<Type, bool> selector = null, 
            Func<Type, Type, bool> serviceTypeSelector = null,
            bool registerSelf = true,
            Action<RegistrationConfigurator> configurator = null) =>
            registrator.RegisterTypes(assembly.CollectTypes(), 
                selector, 
                serviceTypeSelector, 
                registerSelf, 
                configurator);

        /// <summary>
        /// Registers the publicly visible types from an assembly collection into the container.
        /// </summary>
        /// <param name="registrator">The registrator.</param>
        /// <param name="assemblies">The assemblies holding the types to register.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="serviceTypeSelector">The service type selector. Used to filter the service types the actual type bound to.</param>
        /// <param name="registerSelf">If it's true the types will be registered to their own type too.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer RegisterAssemblies(this IDependencyCollectionRegistrator registrator, 
            IEnumerable<Assembly> assemblies, 
            Func<Type, bool> selector = null,
            Func<Type, Type, bool> serviceTypeSelector = null,
            bool registerSelf = true,
            Action<RegistrationConfigurator> configurator = null)
        {
            Shield.EnsureNotNull(assemblies, nameof(assemblies));

            foreach (var assembly in assemblies)
                registrator.RegisterAssembly(assembly, 
                    selector, 
                    serviceTypeSelector, 
                    registerSelf, 
                    configurator);

            return (IStashboxContainer)registrator;
        }

        /// <summary>
        /// Registers the publicly visible types from an assembly which contains a given type into the container.
        /// </summary>
        /// <typeparam name="TFrom">The type the assembly contains.</typeparam>
        /// <param name="registrator">The registrator.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="serviceTypeSelector">The service type selector. Used to filter the service types the actual type bound to.</param>
        /// <param name="registerSelf">If it's true the types will be registered to their own type too.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer RegisterAssemblyContaining<TFrom>(this IDependencyCollectionRegistrator registrator, 
            Func<Type, bool> selector = null,
            Func<Type, Type, bool> serviceTypeSelector = null,
            bool registerSelf = true,
            Action<RegistrationConfigurator> configurator = null)
            where TFrom : class =>
            registrator.RegisterAssemblyContaining(typeof(TFrom), 
                selector, 
                serviceTypeSelector, 
                registerSelf, 
                configurator);

        /// <summary>
        /// Registers the publicly visible types from an assembly which contains a given type into the container.
        /// </summary>
        /// <param name="registrator">The registrator.</param>
        /// <param name="typeFrom">The type the assembly contains.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="serviceTypeSelector">The service type selector. Used to filter the service types the actual type bound to.</param>
        /// <param name="registerSelf">If it's true the types will be registered to their own type too.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer RegisterAssemblyContaining(this IDependencyCollectionRegistrator registrator, 
            Type typeFrom, 
            Func<Type, bool> selector = null,
            Func<Type, Type, bool> serviceTypeSelector = null,
            bool registerSelf = true,
            Action<RegistrationConfigurator> configurator = null) =>
            registrator.RegisterAssembly(typeFrom.GetTypeInfo().Assembly, 
                selector, 
                serviceTypeSelector, 
                registerSelf, 
                configurator);

        /// <summary>
        /// Searches the given assemblies for <see cref="ICompositionRoot"/> implementations and invokes their <see cref="ICompositionRoot.Compose"/> method.
        /// </summary>
        /// <param name="registrator">The registrator.</param>
        /// <param name="assemblies">The assemblies to scan.</param>
        /// <param name="selector">The type selector.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer ComposeAssemblies(this IDependencyCollectionRegistrator registrator, 
            IEnumerable<Assembly> assemblies, 
            Func<Type, bool> selector = null)
        {
            Shield.EnsureNotNull(assemblies, nameof(assemblies));

            foreach (var assembly in assemblies)
                registrator.ComposeAssembly(assembly, selector);

            return (IStashboxContainer)registrator;
        }

        /// <summary>
        /// Composes services by calling the <see cref="ICompositionRoot.Compose"/> method of the given type parameter.
        /// </summary>
        /// <typeparam name="TCompositionRoot">The type of an <see cref="ICompositionRoot"/> implementation.</typeparam>
        /// <param name="registrator">The registrator.</param>
        /// <param name="compositionRootArguments">Optional composition root constructor arguments.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer ComposeBy<TCompositionRoot>(this IDependencyCollectionRegistrator registrator, 
            params object[] compositionRootArguments)
            where TCompositionRoot : class, ICompositionRoot =>
            registrator.ComposeBy(typeof(TCompositionRoot), compositionRootArguments);

        /// <summary>
        /// Searches the given assembly for <see cref="ICompositionRoot"/> implementations and invokes their <see cref="ICompositionRoot.Compose"/> method.
        /// </summary>
        /// <param name="registrator">The registrator.</param>
        /// <param name="assembly">The assembly to scan.</param>
        /// <param name="selector">The type selector.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer ComposeAssembly(this IDependencyCollectionRegistrator registrator, 
            Assembly assembly, 
            Func<Type, bool> selector = null)
        {
            Shield.EnsureNotNull(assembly, nameof(assembly));

            var types = selector == null ? assembly.CollectTypes() : assembly.CollectTypes().Where(selector);
            var compositionRootTypes = types.Where(type => type.IsResolvableType() && type.IsCompositionRoot());

            if (!compositionRootTypes.Any())
                throw new CompositionRootNotFoundException(assembly);

            foreach (var compositionRootType in compositionRootTypes)
                registrator.ComposeBy(compositionRootType);

            return (IStashboxContainer)registrator;
        }

    }
}
