using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules
{
    internal class ScopeNameRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation, IServiceRegistration registration,
            ResolutionContext resolutionContext)
        {
            if (resolutionContext.ScopeNames.IsEmpty && registration.HasScopeName)
                return false;

            return resolutionContext.ScopeNames.IsEmpty || !registration.HasScopeName || registration.CanInjectIntoNamedScope(resolutionContext.ScopeNames);
        }

        public bool ShouldIncrementWeight(TypeInformation typeInformation, IServiceRegistration registration,
            ResolutionContext resolutionContext) => !resolutionContext.ScopeNames.IsEmpty &&
                                                    registration.HasScopeName &&
                                                    registration.CanInjectIntoNamedScope(resolutionContext.ScopeNames);
    }
}
