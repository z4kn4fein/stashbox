using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules
{
    internal class DecoratorRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation, ServiceRegistration registration,
            ResolutionContext resolutionContext) => !resolutionContext.CurrentDecorators.ContainsReference(registration);

        public bool ShouldIncrementWeight(TypeInformation typeInformation, ServiceRegistration registration,
            ResolutionContext resolutionContext) => false;
    }
}
