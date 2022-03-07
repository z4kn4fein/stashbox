using Stashbox.Registration.Fluent;
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
        /// Registers a collection of types mapped to a service type.
        /// </summary>
        /// <param name="typeFrom">The service type.</param>
        /// <param name="types">Types to register.</param>
        /// <param name="selector">The type selector. Used to filter which types should be excluded/included in the registration process.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        IStashboxContainer RegisterTypesAs(Type typeFrom,
            IEnumerable<Type> types,
            Func<Type, bool>? selector = null,
            Action<RegistrationConfigurator>? configurator = null);

        /// <summary>
        /// Registers a collection of types into the container.
        /// </summary>
        /// <param name="types">Types to register.</param>
        /// <param name="selector">The type selector. Used to filter which types should be excluded/included in the registration process.</param>
        /// <param name="serviceTypeSelector">The service type selector. Used to filter which interface or base types the implementation should be mapped to.</param>
        /// <param name="registerSelf">If it's true, the types will be registered to their own type too.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        IStashboxContainer RegisterTypes(IEnumerable<Type> types,
            Func<Type, bool>? selector = null,
            Func<Type, Type, bool>? serviceTypeSelector = null,
            bool registerSelf = true,
            Action<RegistrationConfigurator>? configurator = null);

        /// <summary>
        /// Composes services by calling the <see cref="ICompositionRoot.Compose"/> method of the given root.
        /// </summary>
        /// <param name="compositionRootType">The type of an <see cref="ICompositionRoot"/> implementation.</param>
        /// <param name="compositionRootArguments">Optional composition root constructor arguments.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        IStashboxContainer ComposeBy(Type compositionRootType,
            params object[] compositionRootArguments);

        /// <summary>
        /// Composes services by calling the <see cref="ICompositionRoot.Compose"/> method of the given root.
        /// </summary>
        /// <param name="compositionRoot">The composition root instance.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        IStashboxContainer ComposeBy(ICompositionRoot compositionRoot);
    }
}
