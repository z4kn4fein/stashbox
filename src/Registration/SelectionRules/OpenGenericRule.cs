using Stashbox.Resolution;
using System;

namespace Stashbox.Registration.SelectionRules
{
    internal class OpenGenericRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation,
            ServiceRegistration registration, ResolutionContext resolutionContext) =>
            !typeInformation.Type.IsClosedGenericType() ||
            registration.ImplementationType.SatisfiesGenericConstraintsOf(typeInformation.Type);

        public bool ShouldIncrementWeight(TypeInformation typeInformation,
            ServiceRegistration registration, ResolutionContext resolutionContext) => false;
    }
}
