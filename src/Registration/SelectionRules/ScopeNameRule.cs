using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules
{
    internal class ScopeNameRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation, 
            ServiceRegistration registration, ResolutionContext resolutionContext)
        {
            if (resolutionContext.ScopeNames.Length == 0 && registration.HasScopeName)
                return false;

            return resolutionContext.ScopeNames.Length == 0 ||
                !registration.HasScopeName ||
                resolutionContext.ScopeNames.Contains(registration.NamedScopeRestrictionIdentifier);
        }

        public bool ShouldIncrementWeight(TypeInformation typeInformation, 
            ServiceRegistration registration, ResolutionContext resolutionContext) => 
            resolutionContext.ScopeNames.Length != 0 &&
            registration.HasScopeName &&
            resolutionContext.ScopeNames.First() == registration.NamedScopeRestrictionIdentifier;
    }
}
