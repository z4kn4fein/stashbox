using Stashbox.Registration.Fluent;
using System;

namespace Stashbox
{
    /// <summary>
    /// Represents the extension methods of <see cref="IDecoratorRegistrator"/>.
    /// </summary>
    public static class DecoratorRegistratorExtensions
    {
        /// <summary>
        /// Registers a decorator type into the container with custom configuration.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="registrator">The decorator registrator.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer RegisterDecorator<TFrom, TTo>(this IDecoratorRegistrator registrator, Action<IFluentDecoratorConfigurator> configurator = null)
            where TFrom : class
            where TTo : class, TFrom =>
            registrator.RegisterDecorator(typeof(TFrom), typeof(TTo), configurator);

        /// <summary>
        /// Registers a decorator type into the container with custom configuration.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="registrator">The decorator registrator.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer RegisterDecorator<TFrom>(this IDecoratorRegistrator registrator, Type typeTo, Action<IFluentDecoratorConfigurator> configurator = null)
            where TFrom : class =>
            registrator.RegisterDecorator(typeof(TFrom), typeTo, configurator);

    }
}
