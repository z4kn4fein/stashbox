using Stashbox.Infrastructure;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a scoped lifetime.
    /// </summary>
    public class ScopedLifetime : SingletonLifetime
    {
        /// <inheritdoc />
        public override bool IsScoped => true;

        /// <inheritdoc />
        public override ILifetime Create() => new ScopedLifetime();
    }
}
