using Stashbox.Entity;
using Stashbox.Resolution;

namespace Stashbox.Registration.Filters
{
    internal class ScopeNameFilter : IRegistrationFilter
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation, IServiceRegistration registration,
            ResolutionContext resolutionContext)
        {
            if (resolutionContext.ScopeNames == null && registration.HasScopeName)
                return false;

            return resolutionContext.ScopeNames == null || !registration.HasScopeName || registration.CanInjectIntoNamedScope(resolutionContext.ScopeNames);
        }

        public bool ShouldIncrementWeight(TypeInformation typeInformation, IServiceRegistration registration,
            ResolutionContext resolutionContext) => resolutionContext.ScopeNames != null &&
                                                    registration.HasScopeName &&
                                                    registration.CanInjectIntoNamedScope(resolutionContext.ScopeNames);
    }
}
