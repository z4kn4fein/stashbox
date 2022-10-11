using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules
{
    internal class DecoratorRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation, ServiceRegistration registration,
            ResolutionContext resolutionContext, out bool shouldIncrementWeight)
        {
            shouldIncrementWeight = false;
            return !resolutionContext.CurrentDecorators.ContainsReference(registration);
        }
    }
}
