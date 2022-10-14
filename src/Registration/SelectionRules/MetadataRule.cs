using Stashbox.Resolution;
using System.Collections.Generic;

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
                var metadata = registration.Options.GetOrDefault(RegistrationOption.Metadata);

                shouldIncrementWeight = metadata != null && typeInformation.MetadataType.IsInstanceOfType(metadata);
                return shouldIncrementWeight;
            }

            return true;
        }
    }
}
