using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules;

internal interface IRegistrationSelectionRule
{
    bool IsValidForCurrentRequest(TypeInformation typeInformation,
        ServiceRegistration registration, ResolutionContext resolutionContext, out bool shouldIncrementWeight);
}