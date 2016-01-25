using Stashbox.Entity;
using System;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a registration context. Allows a fluent registration configuration.
    /// </summary>
    public interface IRegistrationContext
    {
        /// <summary>
        /// The type that will be requested.
        /// </summary>
        Type TypeFrom { get; }

        /// <summary>
        /// The type that will be returned.
        /// </summary>
        Type TypeTo { get; }

        /// <summary>
        /// The <see cref="IContainerContext"/> of the <see cref="StashboxContainer"/>
        /// </summary>
        IContainerContext ContainerContext { get; }

        /// <summary>
        /// Sets the lifetime of the registration.
        /// </summary>
        /// <param name="lifetime">An <see cref="ILifetime"/> implementation.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WithLifetime(ILifetime lifetime);

        /// <summary>
        /// Sets the name of the registration.
        /// </summary>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WithName(string name);

        /// <summary>
        /// Sets injection parameters for the registration.
        /// </summary>
        /// <param name="injectionParameters">The injection parameters.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WithInjectionParameters(params InjectionParameter[] injectionParameters);

        /// <summary>
        /// Sets a parameterless factory delegate for the registration.
        /// </summary>
        /// <param name="singleFactory">The factory delegate.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WithFactoryParameters(Func<object> singleFactory);

        /// <summary>
        /// Sets a one parameter factory delegate for the registration.
        /// </summary>
        /// <param name="singleParameterFactory">The one parameter factory delegate.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WithFactoryParameters(Func<object, object> singleParameterFactory);

        /// <summary>
        /// Sets a two parameters factory delegate for the registration.
        /// </summary>
        /// <param name="twoParametersFactory">The two parameters factory delegate.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WithFactoryParameters(Func<object, object, object> twoParametersFactory);

        /// <summary>
        /// Sets a three parameters factory delegate for the registration.
        /// </summary>
        /// <param name="threeParametersFactory">The three parameters factory delegate.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WithFactoryParameters(Func<object, object, object, object> threeParametersFactory);

        /// <summary>
        /// Sets a dependant target condition for the registration.
        /// </summary>
        /// <typeparam name="TTarget">The type of the dependant.</typeparam>
        /// <param name="dependencyName">The name of the dependency.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WhenDependantIs<TTarget>(string dependencyName = null) where TTarget : class;

        /// <summary>
        /// Sets a dependant target condition for the registration.
        /// </summary>
        /// <param name="targetType">The type of the dependant.</param>
        /// <param name="dependencyName">The name of the dependency.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WhenDependantIs(Type targetType, string dependencyName = null);

        /// <summary>
        /// Sets an attribute condition for the registration.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WhenHas<TAttribute>() where TAttribute : Attribute;

        /// <summary>
        /// Sets an attribute condition for the registration.
        /// </summary>
        /// <param name="attributeType">The type of the attribute.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WhenHas(Type attributeType);

        /// <summary>
        /// Sets a generic condition for the registration.
        /// </summary>
        /// <param name="resolutionCondition">The predicate.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext When(Func<TypeInformation, bool> resolutionCondition);

        /// <summary>
        /// Registers the registration into the container.
        /// </summary>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IStashboxContainer Register();

        /// <summary>
        /// Replaces an already registered service.
        /// </summary>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IStashboxContainer ReMap();
    }
}
