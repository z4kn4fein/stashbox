using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules
{
    internal class ConditionRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation, ServiceRegistration registration,
            ResolutionContext resolutionContext) => true;

        public bool ShouldIncrementWeight(TypeInformation typeInformation, ServiceRegistration registration,
            ResolutionContext resolutionContext) =>
            registration.HasCondition && registration.IsUsableForCurrentContext(typeInformation);
    }
}
