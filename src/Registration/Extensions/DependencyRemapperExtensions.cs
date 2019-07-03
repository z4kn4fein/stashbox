using Stashbox.Registration.Fluent;
using System;

namespace Stashbox
{
    /// <summary>
    /// Represents the extension methods of <see cref="IDependencyReMapper"/>.
    /// </summary>
    public static class DependencyReMapperExtensions
    {
        /// <summary>
        /// Replaces an existing registration mapping.
        /// </summary>
        /// <param name="reMapper">The remapper.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer ReMap(this IDependencyReMapper reMapper, Type typeTo, Action<IFluentServiceConfigurator> configurator = null) =>
            reMapper.ReMap(typeTo, typeTo, configurator);

        /// <summary>
        /// Replaces an existing decorator mapping.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="reMapper">The remapper.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer ReMapDecorator<TFrom, TTo>(this IDependencyReMapper reMapper, Action<IFluentDecoratorConfigurator> configurator = null)
            where TFrom : class
            where TTo : class, TFrom =>
            reMapper.ReMapDecorator(typeof(TFrom), typeof(TTo), configurator);

        /// <summary>
        /// Replaces an existing decorator mapping.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="reMapper">The remapper.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer ReMapDecorator<TFrom>(this IDependencyReMapper reMapper, Type typeTo, Action<IFluentDecoratorConfigurator> configurator = null)
            where TFrom : class =>
            reMapper.ReMapDecorator(typeof(TFrom), typeTo, configurator);

    }
}
