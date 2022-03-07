using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules
{
    internal class MetadataRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation,
            ServiceRegistration registration, ResolutionContext resolutionContext)
        {
            if (typeInformation.MetadataType != null)
                return registration.Metadata != null &&
                    typeInformation.MetadataType.IsInstanceOfType(registration.Metadata);

            return true;
        }

        public bool ShouldIncrementWeight(TypeInformation typeInformation,
            ServiceRegistration registration, ResolutionContext resolutionContext) =>
            typeInformation.MetadataType != null &&
            registration.Metadata != null &&
            typeInformation.MetadataType.IsInstanceOfType(registration.Metadata);
    }
}
