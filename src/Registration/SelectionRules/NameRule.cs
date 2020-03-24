using System;
using Stashbox.Entity;
using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules
{
    internal class NameRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation, IServiceRegistration registration,
            ResolutionContext resolutionContext)
        {
            if (typeInformation.DependencyName != null &&
                !registration.RegistrationName.Equals(typeInformation.DependencyName) &&
                !registration.IsResolvableByUnnamedRequest)
                return false;

            return typeInformation.DependencyName != null || registration.IsResolvableByUnnamedRequest;
        }

        public bool ShouldIncrementWeight(TypeInformation typeInformation, IServiceRegistration registration,
            ResolutionContext resolutionContext) => typeInformation.DependencyName != null &&
                                                    registration.RegistrationName.Equals(typeInformation.DependencyName);
    }
}
