using System;

namespace Stashbox
{
    /// <summary>
    /// Represents a factory registrator.
    /// </summary>
    public interface IFuncRegistrator
    {
        /// <summary>
        /// Registers a service with a factory resolver.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="name">The name of the factory registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer RegisterFunc<TService>(Func<IDependencyResolver, TService> factory, string name = null);

        /// <summary>
        /// Registers a service with a factory resolver.
        /// </summary>
        /// <typeparam name="T1">The first parameter of the factory.</typeparam>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="name">The name of the factory registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer RegisterFunc<T1, TService>(Func<T1, IDependencyResolver, TService> factory, string name = null);

        /// <summary>
        /// Registers a service with a factory resolver.
        /// </summary>
        /// <typeparam name="T1">The first parameter of the factory.</typeparam>
        /// <typeparam name="T2">The second parameter of the factory.</typeparam>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="name">The name of the factory registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer RegisterFunc<T1, T2, TService>(Func<T1, T2, IDependencyResolver, TService> factory, string name = null);

        /// <summary>
        /// Registers a service with a factory resolver.
        /// </summary>
        /// <typeparam name="T1">The first parameter of the factory.</typeparam>
        /// <typeparam name="T2">The second parameter of the factory.</typeparam>
        /// <typeparam name="T3">The third parameter of the factory.</typeparam>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="name">The name of the factory registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer RegisterFunc<T1, T2, T3, TService>(Func<T1, T2, T3, IDependencyResolver, TService> factory, string name = null);

        /// <summary>
        /// Registers a service with a factory resolver.
        /// </summary>
        /// <typeparam name="T1">The first parameter of the factory.</typeparam>
        /// <typeparam name="T2">The second parameter of the factory.</typeparam>
        /// <typeparam name="T3">The third parameter of the factory.</typeparam>
        /// <typeparam name="T4">The fourth parameter of the factory.</typeparam>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="name">The name of the factory registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        IStashboxContainer RegisterFunc<T1, T2, T3, T4, TService>(Func<T1, T2, T3, T4, IDependencyResolver, TService> factory, string name = null);
    }
}
