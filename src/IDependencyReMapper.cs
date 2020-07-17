using Stashbox.Registration.Fluent;
using System;

namespace Stashbox
{
    /// <summary>
    /// Represents a dependency remapper.
    /// </summary>
    public interface IDependencyReMapper
    {
        /// <summary>
        /// Replaces an existing registration mapping.
        /// </summary>
        /// <typeparam name="TFrom">Type service type to re-map.</typeparam>
        /// <typeparam name="TTo">Type implementation type to re-map.</typeparam>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer ReMap<TFrom, TTo>(Action<RegistrationConfigurator<TFrom, TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Replaces an existing registration mapping.
        /// </summary>
        /// <typeparam name="TFrom">Type service type to re-map.</typeparam>
        /// <param name="typeTo">Type implementation type to re-map.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer ReMap<TFrom>(Type typeTo, Action<RegistrationConfigurator<TFrom, TFrom>> configurator = null)
            where TFrom : class;

        /// <summary>
        /// Replaces an existing registration mapping.
        /// </summary>
        /// <param name="typeFrom">Type service type to re-map.</param>
        /// <param name="typeTo">Type implementation type to re-map.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer ReMap(Type typeFrom, Type typeTo, Action<RegistrationConfigurator> configurator = null);

        /// <summary>
        /// Replaces an existing registration mapping.
        /// </summary>
        /// <typeparam name="TTo">Type implementation type to re-map.</typeparam>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer ReMap<TTo>(Action<RegistrationConfigurator<TTo, TTo>> configurator = null)
             where TTo : class;

        /// <summary>
        /// Replaces an existing decorator mapping.
        /// </summary>
        /// <param name="typeFrom">Type service type to re-map.</param>
        /// <param name="typeTo">Type implementation type to re-map.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer ReMapDecorator(Type typeFrom, Type typeTo, Action<DecoratorConfigurator> configurator = null);

        /// <summary>
        /// Replaces an existing decorator mapping.
        /// </summary>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer ReMapDecorator<TFrom, TTo>(Action<DecoratorConfigurator<TFrom, TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Replaces an existing decorator mapping.
        /// </summary>
        /// <param name="typeTo">Type implementation type to re-map.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer ReMapDecorator<TFrom>(Type typeTo, Action<DecoratorConfigurator<TFrom, TFrom>> configurator = null)
            where TFrom : class;
    }
}
