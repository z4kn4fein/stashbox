using Stashbox.Lifetime;
using System;
using System.Linq;
using Stashbox.Utils;

namespace Stashbox.Registration.Fluent;

/// <summary>
/// Represents the generic base of the fluent registration api.
/// </summary>
/// <typeparam name="TConfigurator"></typeparam>
public class BaseDecoratorConfigurator<TConfigurator> : BaseFluentConfigurator<TConfigurator>
    where TConfigurator : BaseDecoratorConfigurator<TConfigurator>
{
    internal BaseDecoratorConfigurator(Type serviceType, Type implementationType, object? name)
        : base(serviceType, implementationType, name, Lifetimes.Empty, true)
    { }

    /// <summary>
    /// Sets a decorated target condition for the registration.
    /// </summary>
    /// <typeparam name="TTarget">The type of the parent.</typeparam>
    /// <returns>The fluent configurator.</returns>
    public TConfigurator WhenDecoratedServiceIs<TTarget>() where TTarget : class => this.WhenDecoratedServiceIs(TypeCache<TTarget>.Type);

    /// <summary>
    /// Sets a decorated target condition for the registration.
    /// </summary>
    /// <param name="targetType">The type of the decorated service.</param>
    /// <returns>The fluent configurator.</returns>
    public TConfigurator WhenDecoratedServiceIs(Type targetType) => this.When(typeInfo => typeInfo.Type == targetType);

    /// <summary>
    /// Sets a decorated target condition for the registration.
    /// </summary>
    /// <param name="name">The name of the decorated service.</param>
    /// <returns>The fluent configurator.</returns>
    public TConfigurator WhenDecoratedServiceIs(object name) => this.When(typeInfo => name.Equals(typeInfo.DependencyName));

    /// <summary>
    /// Sets an attribute condition that the decorated target has to satisfy.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <returns>The fluent configurator.</returns>
    public TConfigurator WhenDecoratedServiceHas<TAttribute>() where TAttribute : Attribute => this.WhenDecoratedServiceHas(TypeCache<TAttribute>.Type);

    /// <summary>
    /// Sets an attribute condition that the decorated target has to satisfy.
    /// </summary>
    /// <param name="attributeType">The type of the attribute.</param>
    /// <returns>The fluent configurator.</returns>
    public TConfigurator WhenDecoratedServiceHas(Type attributeType) =>
        this.When(t => t.Type.GetCustomAttributes(attributeType, false).FirstOrDefault() != null);
}