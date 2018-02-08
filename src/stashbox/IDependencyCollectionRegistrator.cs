using Stashbox.Registration;
using System;
using System.Collections.Generic;

namespace Stashbox
{
    /// <summary>
    /// Represents a dependency collection registrator.
    /// </summary>
    public interface IDependencyCollectionRegistrator
    {
        /// <summary>
        /// Registers types into the container mapped to an interface type.
        /// </summary>
        /// <param name="typeFrom">The interface type.</param>
        /// <param name="types">Types to register.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer RegisterTypesAs(Type typeFrom, IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null);

        /// <summary>
        /// Registers types into the container.
        /// </summary>
        /// <param name="types">Types to register.</param>
        /// <param name="selector">The type selector.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer RegisterTypes(IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null);

        /// <summary>
        /// Composes services by calling the <see cref="ICompositionRoot.Compose"/> method of the given type.
        /// </summary>
        /// <param name="compositionRootType">The type of an <see cref="ICompositionRoot"/> implementation.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer ComposeBy(Type compositionRootType);
    }
}
