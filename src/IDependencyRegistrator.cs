using Stashbox.Registration.Fluent;
using System;

namespace Stashbox
{
    /// <summary>
    /// Represents a dependency registrator.
    /// </summary>
    public interface IDependencyRegistrator
    {
        /// <summary>
        /// Registers a type into the container with custom configuration.
        /// </summary>
        /// <typeparam name="TFrom">The service type.</typeparam>
        /// <typeparam name="TTo">The implementation type.</typeparam>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        IStashboxContainer Register<TFrom, TTo>(Action<RegistrationConfigurator<TFrom, TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom;

        /// <summary>
        /// Registers a type into the container with custom configuration.
        /// </summary>
        /// <typeparam name="TFrom">The service type.</typeparam>
        /// <param name="typeTo">The implementation type.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        IStashboxContainer Register<TFrom>(Type typeTo, Action<RegistrationConfigurator<TFrom, TFrom>> configurator = null)
            where TFrom : class;

        /// <summary>
        /// Registers a type into the container with custom configuration.
        /// </summary>
        /// <param name="typeFrom">The service type.</param>
        /// <param name="typeTo">The implementation type.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        IStashboxContainer Register(Type typeFrom, Type typeTo, Action<RegistrationConfigurator> configurator = null);

        /// <summary>
        /// Registers a type into the container with custom configuration.
        /// </summary>
        /// <typeparam name="TTo">The service/implementation type.</typeparam>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        IStashboxContainer Register<TTo>(Action<RegistrationConfigurator<TTo, TTo>> configurator = null)
             where TTo : class;

        /// <summary>
        /// Registers a type into the container with custom configuration.
        /// </summary>
        /// <param name="typeTo">The service/implementation type.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        IStashboxContainer Register(Type typeTo, Action<RegistrationConfigurator> configurator = null);

        /// <summary>
        /// Registers an already constructed instance into the container.
        /// </summary>
        /// <typeparam name="TInstance">The service type.</typeparam>
        /// <param name="instance">The constructed instance.</param>
        /// <param name="name">The name of the registration.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from disposal tracking.</param>
        /// <param name="finalizerDelegate">The cleanup delegate to call before dispose.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        IStashboxContainer RegisterInstance<TInstance>(TInstance instance, object name = null, bool withoutDisposalTracking = false, Action<TInstance> finalizerDelegate = null)
            where TInstance : class;

        /// <summary>
        /// Registers an already constructed instance into the container.
        /// </summary>
        /// <param name="instance">The constructed instance.</param>
        /// <param name="serviceType">The service type.</param>
        /// <param name="name">The name of the registration.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from disposal tracking.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        IStashboxContainer RegisterInstance(object instance, Type serviceType, object name = null, bool withoutDisposalTracking = false);

        /// <summary>
        /// Registers an already constructed instance, but the container will perform injections and extensions on it.
        /// </summary>
        /// <typeparam name="TInstance">The service type.</typeparam>
        /// <param name="instance">The constructed instance.</param>
        /// <param name="name">The name of the registration.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from disposal tracking.</param>
        /// <param name="finalizerDelegate">The cleanup delegate to call before dispose.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        IStashboxContainer WireUp<TInstance>(TInstance instance, object name = null, bool withoutDisposalTracking = false, Action<TInstance> finalizerDelegate = null)
            where TInstance : class;

        /// <summary>
        /// Registers an already constructed instance, but the container will perform injections and extensions on it.
        /// </summary>
        /// <param name="instance">The constructed instance.</param>
        /// <param name="serviceType">The service type.</param>
        /// <param name="name">The name of the registration.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from disposal tracking.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        IStashboxContainer WireUp(object instance, Type serviceType, object name = null, bool withoutDisposalTracking = false);
    }
}
