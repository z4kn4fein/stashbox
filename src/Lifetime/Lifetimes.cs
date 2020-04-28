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
        /// NamedScope lifetime.
        /// </summary>
        public static readonly LifetimeDescriptor NamedScope = new NamedScopeLifetime();

        /// <summary>
        /// PerRequest lifetime.
        /// </summary>
        public static readonly LifetimeDescriptor PerRequest = new ResolutionRequestLifetime();
    }
}
