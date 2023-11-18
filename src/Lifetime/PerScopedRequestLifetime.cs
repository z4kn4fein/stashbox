namespace Stashbox.Lifetime;

/// <summary>
/// Represents a lifetime that re-uses an instance within a scoped service's resolution tree.
/// </summary>
public class PerScopedRequestLifetime : TransientLifetime
{
    /// <inheritdoc />
    internal override bool StoreResultInLocalVariable => true;
}