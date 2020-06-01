using Stashbox.Resolution;
using System;

namespace Stashbox.Registration.SelectionRules
{
    internal class GenericRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation,
            ServiceRegistration registration,
            ResolutionContext resolutionContext) =>
            !typeInformation.Type.IsClosedGenericType() || typeInformation.Type.SatisfiesGenericConstraintsOf(registration.ImplementationTypeInfo);

        public bool ShouldIncrementWeight(TypeInformation typeInformation, ServiceRegistration registration,
            ResolutionContext resolutionContext) => false;
    }
}
