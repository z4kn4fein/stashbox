using System;
using Stashbox.Entity;
using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules
{
    internal class GenericRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation,
            IServiceRegistration registration,
            ResolutionContext resolutionContext) => 
            !typeInformation.Type.IsClosedGenericType() || registration.ValidateGenericConstraints(typeInformation.Type);

        public bool ShouldIncrementWeight(TypeInformation typeInformation, IServiceRegistration registration,
            ResolutionContext resolutionContext) => false;
    }
}
