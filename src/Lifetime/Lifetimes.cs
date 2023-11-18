namespace Stashbox.Lifetime;

/// <summary>
/// Contains all the built-in lifetime managers.
/// </summary>
public static class Lifetimes
{
    /// <summary>
    /// Transient lifetime.
    /// </summary>
    public static readonly LifetimeDescriptor Transient = new TransientLifetime();

    /// <summary>
    /// Singleton lifetime.
    /// </summary>
    public static readonly LifetimeDescriptor Singleton = new SingletonLifetime();

    /// <summary>
    /// Scoped lifetime.
    /// </summary>
    public static readonly LifetimeDescriptor Scoped = new ScopedLifetime();

    /// <summary>
    /// Per scoped request lifetime, that re-uses the produced instance within a scoped service's resolution tree.
    /// </summary>
    public static readonly LifetimeDescriptor PerScopedRequest = new PerScopedRequestLifetime();

    /// <summary>
    /// Per resolution request lifetime.
    /// </summary>
    public static readonly LifetimeDescriptor PerRequest = new PerRequestLifetime();

    /// <summary>
    /// Produces a NamedScope lifetime.
    /// </summary>
    /// <param name="name">The name of the scope.</param>
    /// <returns>A named-scope lifetime.</returns>
    public static LifetimeDescriptor NamedScope(object name) => new NamedScopeLifetime(name);

    /// <summary>
    /// Produces a lifetime that aligns to the lifetime of the resolved service's dependencies.
    /// When the underlying service has a dependency with a higher lifespan, this lifetime will inherit that lifespan up to a given boundary.
    /// </summary>
    /// <param name="boundaryLifetime">The lifetime that represents a boundary which the derived lifetime must not exceed.</param>
    /// <returns>An auto lifetime.</returns>
    public static LifetimeDescriptor Auto(LifetimeDescriptor boundaryLifetime) => new AutoLifetime(boundaryLifetime);

    internal static readonly LifetimeDescriptor Empty = new EmptyLifetime();
}