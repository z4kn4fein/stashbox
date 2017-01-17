using Stashbox.Infrastructure;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a transient lifetime manager.
    /// </summary>
    public class TransientLifetime : LifetimeBase
    {
        /// <inheritdoc />
        public override ILifetime Create()
        {
            return new TransientLifetime();
        }
    }
}
