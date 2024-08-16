using System;
using Stashbox.Utils;

namespace Stashbox;

/// <summary>
/// Represents a specialized resolution override.
/// </summary>
public class Override
{
    private Override(object instance, object? dependencyName, Type type)
    {
        this.Instance = instance;
        this.DependencyName = dependencyName;
        this.Type = type;
    }

    /// <summary>
    /// The name of the override used for named resolution.
    /// </summary>
    public object? DependencyName { get; }

    /// <summary>
    /// The instance used as dependency override.
    /// </summary>
    public object Instance { get; }

    /// <summary>
    /// The type of the dependency override.
    /// </summary>
    public Type Type { get; }
    
    /// <summary>
    /// Creates a new <see cref="Override"/> instance.
    /// </summary>
    /// <param name="instance">The instance used as dependency override.</param>
    /// <param name="name">The optional name of the instance used as dependency override.</param>
    /// <returns>The constructed override instance.</returns>
    public static Override Of<TType>(TType instance, object? name = null) where TType: notnull => new(instance, name, TypeCache<TType>.Type);

    /// <summary>
    /// Creates a new <see cref="Override"/> instance.
    /// </summary>
    /// <param name="type">The type of the override.</param>
    /// <param name="instance">The instance used as dependency override.</param>
    /// <param name="name">The optional name of the instance used as dependency override.</param>
    /// <returns>The constructed override instance.</returns>
    public static Override Of(Type type, object instance, object? name = null) => new(instance, name, type);
}