namespace Stashbox.Resolution;

internal static class ResolutionBehaviorExtensions
{
    public static bool Has(this ResolutionBehavior resolutionBehavior, ResolutionBehavior toCheck) =>
        (resolutionBehavior & toCheck) == toCheck;
}