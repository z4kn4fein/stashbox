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
        public static LifetimeDescriptor Transient = new TransientLifetime();

        /// <summary>
        /// Singleton lifetime.
        /// </summary>
        public static LifetimeDescriptor Singleton = new SingletonLifetime();

        /// <summary>
        /// Scoped lifetime.
        /// </summary>
        public static LifetimeDescriptor Scoped = new ScopedLifetime();

        /// <summary>
        /// NamedScope lifetime.
        /// </summary>
        public static LifetimeDescriptor NamedScope = new NamedScopeLifetime();

        /// <summary>
        /// PerRequest lifetime.
        /// </summary>
        public static LifetimeDescriptor PerRequest = new ResolutionRequestLifetime();
    }
}
