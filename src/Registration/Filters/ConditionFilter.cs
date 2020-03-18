 using Stashbox.Entity;
using Stashbox.Resolution;

namespace Stashbox.Registration.Filters
{
    internal class ConditionFilter : IRegistrationFilter
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation, IServiceRegistration registration,
            ResolutionContext resolutionContext) => true;

        public bool ShouldIncrementWeight(TypeInformation typeInformation, IServiceRegistration registration,
            ResolutionContext resolutionContext) =>
            registration.HasCondition && registration.IsUsableForCurrentContext(typeInformation);
    }
}
