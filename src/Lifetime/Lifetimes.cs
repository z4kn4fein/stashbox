namespace Stashbox.Lifetime
{
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
        public static LifetimeDescriptor NamedScope(object name) => new NamedScopeLifetime(name);
    }
}
