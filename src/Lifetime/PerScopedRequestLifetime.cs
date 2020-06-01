namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a per resolution request lifetime.
    /// </summary>
    public class PerScopedRequestLifetime : TransientLifetime
    {
        /// <inheritdoc />
        protected override string Name => nameof(PerScopedRequestLifetime);

        /// <inheritdoc />
        private protected override bool StoreResultInLocalVariable => true;
    }
}
