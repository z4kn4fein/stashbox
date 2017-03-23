using Stashbox.Infrastructure;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a transient lifetime.
    /// </summary>
    public class TransientLifetime : LifetimeBase
    {
        /// <inheritdoc />
        public override bool IsTransient => true;

        /// <inheritdoc />
        public override ILifetime Create() => new TransientLifetime();
    }
}
