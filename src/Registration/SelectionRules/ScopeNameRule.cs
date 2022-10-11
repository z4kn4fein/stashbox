using Stashbox.Lifetime;
using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules
{
    internal class ScopeNameRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation,
            ServiceRegistration registration, ResolutionContext resolutionContext, out bool shouldIncrementWeight)
        {
            if (registration.Lifetime is not NamedScopeLifetime namedScopeLifetime)
            {
                shouldIncrementWeight = false;
                return true;
            }

            shouldIncrementWeight = false;
            if (resolutionContext.ScopeNames.Length == 0)
                return false;

            shouldIncrementWeight = resolutionContext.ScopeNames.First() == namedScopeLifetime.ScopeName;
            return resolutionContext.ScopeNames.Contains(namedScopeLifetime.ScopeName);
        }
    }
}
