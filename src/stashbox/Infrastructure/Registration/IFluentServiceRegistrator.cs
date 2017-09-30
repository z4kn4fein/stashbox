using System;
using Stashbox.Entity;

namespace Stashbox.Infrastructure.Registration
{
    /// <summary>
    /// Represents a generic fluent service registrator.
    /// </summary>
    public interface IFluentServiceRegistrator<out TService> : IFluentServiceRegistrator
    {
        /// <summary>
        /// Sets a delegate which will be called when the container is being disposed.
        /// </summary>
        /// <param name="finalizer">The cleanup delegate.</param>
        /// <returns>The <see cref="IFluentServiceRegistrator{TService}"/> which on this method was called.</returns>
        IFluentServiceRegistrator<TService> WithFinalizer(Action<TService> finalizer);

        /// <summary>
        /// Sets a delegate which will be called when the service is being constructed.
        /// </summary>
        /// <param name="initializer">The initializer delegate.</param>
        /// <returns>The <see cref="IFluentServiceRegistrator{TService}"/> which on this method was called.</returns>
        IFluentServiceRegistrator<TService> WithInitializer(Action<TService> initializer);
    }

    /// <summary>
    /// Represents a fluent service registrator.
    /// </summary>
    public interface IFluentServiceRegistrator : IBaseFluentRegistrator<IFluentServiceRegistrator>
    {
        /// <summary>
        /// Sets the lifetime of the registration.
        /// </summary>
        /// <param name="lifetime">An <see cref="ILifetime"/> implementation.</param>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator WithLifetime(ILifetime lifetime);

        /// <summary>
        /// Sets a scoped lifetime for the registration.
        /// </summary>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator WithScopedLifetime();

        /// <summary>
        /// Sets a singleton lifetime for the registration.
        /// </summary>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator WithSingletonLifetime();

        /// <summary>
        /// Sets the name of the registration.
        /// </summary>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator WithName(object name);

        /// <summary>
        /// Sets a parameterless factory delegate for the registration.
        /// </summary>
        /// <param name="singleFactory">The factory delegate.</param>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator WithFactory(Func<object> singleFactory);

        /// <summary>
        /// Sets a container factory delegate for the registration.
        /// </summary>
        /// <param name="containerFactory">The container factory delegate.</param>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator WithFactory(Func<IDependencyResolver, object> containerFactory);

        /// <summary>
        /// Sets an instance as the resolution target of the registration.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator WithInstance(object instance);

        /// <summary>
        /// Sets a dependant target condition for the registration.
        /// </summary>
        /// <typeparam name="TTarget">The type of the dependant.</typeparam>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator WhenDependantIs<TTarget>() where TTarget : class;

        /// <summary>
        /// Sets a dependant target condition for the registration.
        /// </summary>
        /// <param name="targetType">The type of the dependant.</param>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator WhenDependantIs(Type targetType);

        /// <summary>
        /// Sets an attribute condition for the registration.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator WhenHas<TAttribute>() where TAttribute : Attribute;

        /// <summary>
        /// Sets an attribute condition for the registration.
        /// </summary>
        /// <param name="attributeType">The type of the attribute.</param>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator WhenHas(Type attributeType);

        /// <summary>
        /// Sets a generic condition for the registration.
        /// </summary>
        /// <param name="resolutionCondition">The predicate.</param>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator When(Func<TypeInformation, bool> resolutionCondition);

        /// <summary>
        /// Registers the given service by all of it's implemented types.
        /// </summary>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator AsImplementedTypes();
    }
}
