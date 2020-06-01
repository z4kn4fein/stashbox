using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules
{
    internal interface IRegistrationSelectionRule
    {
        bool IsValidForCurrentRequest(TypeInformation typeInformation,
            IServiceRegistration registration, ResolutionContext resolutionContext);

        bool ShouldIncrementWeight(TypeInformation typeInformation,
            IServiceRegistration registration, ResolutionContext resolutionContext);
    }
}
