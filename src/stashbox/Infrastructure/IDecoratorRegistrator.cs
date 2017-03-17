using System;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a decorator registrator.
    /// </summary>
    public interface IDecoratorRegistrator
    {
        /// <summary>
        /// Prepares a decorator type for registration.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <returns>The created <see cref="IDecoratorRegistrationContext"/> which allows further configurations.</returns>
        IDecoratorRegistrationContext PrepareDecorator<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Prepares a decorator type for registration.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <returns>The created <see cref="IDecoratorRegistrationContext"/> which allows further configurations.</returns>
        IDecoratorRegistrationContext PrepareDecorator<TFrom>(Type typeTo)
            where TFrom : class;

        /// <summary>
        /// Prepares a decorator type for registration.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <returns>The created <see cref="IDecoratorRegistrationContext"/> which allows further configurations.</returns>
        IDecoratorRegistrationContext PrepareDecorator(Type typeFrom, Type typeTo);

        /// <summary>
        /// Registers a decorator type into the container.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <returns>The <see cref="IDecoratorRegistrator"/> which on this method was called.</returns>
        IDecoratorRegistrator RegisterDecorator<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Registers a decorator type into the container.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <returns>The <see cref="IDecoratorRegistrator"/> which on this method was called.</returns>
        IDecoratorRegistrator RegisterDecorator<TFrom>(Type typeTo)
            where TFrom : class;

        /// <summary>
        /// Registers a decorator type into the container.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <returns>The <see cref="IDecoratorRegistrator"/> which on this method was called.</returns>
        IDecoratorRegistrator RegisterDecorator(Type typeFrom, Type typeTo);
    }
}
