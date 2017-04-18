using System;
using System.Collections.Generic;
using Stashbox.Configuration;
using Stashbox.Entity;

namespace Stashbox.Infrastructure.Registration
{
    /// <summary>
    /// Represents a fluent service registrator.
    /// </summary>
    public interface IFluentServiceRegistrator
    {
        /// <summary>
        /// The service type.
        /// </summary>
        Type ServiceType { get; }

        /// <summary>
        /// The implementation type.
        /// </summary>
        Type ImplementationType { get; }

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
        IFluentServiceRegistrator WithName(string name);

        /// <summary>
        /// Sets injection parameters for the registration.
        /// </summary>
        /// <param name="injectionParameters">The injection parameters.</param>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator WithInjectionParameters(params InjectionParameter[] injectionParameters);

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
        /// Enables auto member injection on the registration.
        /// </summary>
        /// <param name="rule">The auto member injection rule.</param>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator WithAutoMemberInjection(Rules.AutoMemberInjection rule = Rules.AutoMemberInjection.PropertiesWithPublicSetter);

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        /// <param name="rule">The constructor selection rule.</param>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator WithConstructorSelectionRule(Func<IEnumerable<ConstructorInformation>, IEnumerable<ConstructorInformation>> rule);

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
        /// Tells the container that it shouldn't track the resolved transient object for disposal.
        /// </summary>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator WithoutDisposalTracking();

        /// <summary>
        /// Tells the container that it should replace an existing registration with this one.
        /// </summary>
        /// <returns>The <see cref="IFluentServiceRegistrator"/> which on this method was called.</returns>
        IFluentServiceRegistrator ReplaceExisting();
    }
}
