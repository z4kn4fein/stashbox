using Stashbox.Entity;
using Stashbox.Lifetime;
using System;
using System.Linq.Expressions;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// Represents the generic fluent service registration api.
    /// </summary>
    public interface IFluentServiceConfigurator<TService, TConfigurator> : IFluentServiceConfigurator<TConfigurator>
        where TConfigurator : IFluentServiceConfigurator<TService, TConfigurator>
    {
        /// <summary>
        /// Sets a delegate which will be called when the container is being disposed.
        /// </summary>
        /// <param name="finalizer">The cleanup delegate.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator WithFinalizer(Action<TService> finalizer);

        /// <summary>
        /// Sets a delegate which will be called when the service is being constructed.
        /// </summary>
        /// <param name="initializer">The initializer delegate.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator WithInitializer(Action<TService, IDependencyResolver> initializer);

        /// <summary>
        /// Set a member (property / field) as a dependency should be filled by the container.
        /// </summary>
        /// <param name="expression">The member expression.</param>
        /// <param name="dependencyName">The name of the dependency.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator InjectMember<TResult>(Expression<Func<TService, TResult>> expression, object dependencyName = null);

        /// <summary>
        /// Binds a constructor or method parameter to a named registration, so the container will perform a named resolution on the bound dependency.  
        /// </summary>
        /// <param name="dependencyName">The name of the bound named registration.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator WithDependencyBinding<TDependency>(object dependencyName);

        /// <summary>
        /// Binds the currently configured registration to an additional service type.
        /// </summary>
        /// <returns>The configurator itself.</returns>
        TConfigurator AsServiceAlso<TAdditionalService>();
    }

    /// <summary>
    /// Represents the fluent service registraton api.
    /// </summary>
    public interface IFluentServiceConfigurator<TConfigurator> : IBaseFluentConfigurator<TConfigurator>
        where TConfigurator : IFluentServiceConfigurator<TConfigurator>
    {
        /// <summary>
        /// Sets the lifetime of the registration.
        /// </summary>
        /// <param name="lifetime">An <see cref="ILifetime"/> implementation.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator WithLifetime(ILifetime lifetime);

        /// <summary>
        /// Sets a scoped lifetime for the registration.
        /// </summary>
        /// <returns>The configurator itself.</returns>
        TConfigurator WithScopedLifetime();

        /// <summary>
        /// Sets a singleton lifetime for the registration.
        /// </summary>
        /// <returns>The configurator itself.</returns>
        TConfigurator WithSingletonLifetime();

        /// <summary>
        /// Sets the name of the registration.
        /// </summary>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator WithName(object name);

        /// <summary>
        /// Sets a parameterless factory delegate for the registration.
        /// </summary>
        /// <param name="singleFactory">The factory delegate.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator WithFactory(Func<object> singleFactory);

        /// <summary>
        /// Sets a container factory delegate for the registration.
        /// </summary>
        /// <param name="containerFactory">The container factory delegate.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator WithFactory(Func<IDependencyResolver, object> containerFactory);

        /// <summary>
        /// Sets an instance as the resolution target of the registration.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="wireUp">If true, the instance will be wired into the container, it will perform member and method injection on it.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator WithInstance(object instance, bool wireUp = false);

        /// <summary>
        /// Sets a parent target condition for the registration.
        /// </summary>
        /// <typeparam name="TTarget">The type of the parent.</typeparam>
        /// <returns>The configurator itself.</returns>
        TConfigurator WhenDependantIs<TTarget>() where TTarget : class;

        /// <summary>
        /// Sets a parent target condition for the registration.
        /// </summary>
        /// <param name="targetType">The type of the parent.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator WhenDependantIs(Type targetType);

        /// <summary>
        /// Sets an attribute condition for the registration.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <returns>The configurator itself.</returns>
        TConfigurator WhenHas<TAttribute>() where TAttribute : Attribute;

        /// <summary>
        /// Sets an attribute condition for the registration.
        /// </summary>
        /// <param name="attributeType">The type of the attribute.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator WhenHas(Type attributeType);

        /// <summary>
        /// Sets a generic condition for the registration.
        /// </summary>
        /// <param name="resolutionCondition">The predicate.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator When(Func<TypeInformation, bool> resolutionCondition);

        /// <summary>
        /// Registers the given service by all of it's implemented types.
        /// </summary>
        /// <returns>The configurator itself.</returns>
        TConfigurator AsImplementedTypes();

        /// <summary>
        /// Binds the currently configured registration to an additional service type.
        /// </summary>
        /// <param name="serviceType">The additional service type.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator AsServiceAlso(Type serviceType);

        /// <summary>
        /// Sets a scope name condition for the registration, it will be used only when a scope with the given name requests it.
        /// </summary>
        /// <param name="scopeName">The name of the scope.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator InNamedScope(object scopeName);

        /// <summary>
        /// It means this registration would be used as a logical scope for it's dependencies, the dependencies registered with the <see cref="InNamedScope"/> and with the same name as it's param will be preffered during reolution.
        /// </summary>
        /// <param name="scopeName">The name of the scope.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator DefinesScope(object scopeName);

        /// <summary>
        /// Sets the lifetime to <see cref="ResolutionRequestLifetime"/>. The container will inject this registration in a singleton per resolution request manner.
        /// </summary>
        /// <returns>The configurator itself.</returns>
        TConfigurator WithPerResolutionRequestLifetime();
    }
}
