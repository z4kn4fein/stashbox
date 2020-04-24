namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a per resolution request lifetime.
    /// </summary>
    public class ResolutionRequestLifetime : TransientLifetime
    {
        /// <inheritdoc />
        protected override bool ShouldStoreResultInLocalVariable => true;
    }
}
