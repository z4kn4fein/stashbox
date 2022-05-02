using Stashbox.Registration.ServiceRegistrations;
using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules
{
    internal class MetadataRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation,
            ServiceRegistration registration, ResolutionContext resolutionContext, out bool shouldIncrementWeight)
        {
            shouldIncrementWeight = false;
            if (typeInformation.MetadataType != null)
            {
                if(registration is not ComplexRegistration complexRegistration)
                    return false;

                shouldIncrementWeight = complexRegistration.Metadata != null && typeInformation.MetadataType.IsInstanceOfType(complexRegistration.Metadata);
                return shouldIncrementWeight;
            }

            return true;
        }
    }
}
