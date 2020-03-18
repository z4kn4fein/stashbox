using System;
using Stashbox.Entity;
using Stashbox.Resolution;

namespace Stashbox.Registration.Filters
{
    internal class NameFilter : IRegistrationFilter
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation, IServiceRegistration registration,
            ResolutionContext resolutionContext)
        {
            if (typeInformation.DependencyName != null &&
                !registration.RegistrationId.Equals(typeInformation.DependencyName) &&
                !registration.IsResolvableByUnnamedRequest)
                return false;

            return typeInformation.DependencyName != null || registration.IsResolvableByUnnamedRequest;
        }

        public bool ShouldIncrementWeight(TypeInformation typeInformation, IServiceRegistration registration,
            ResolutionContext resolutionContext) => typeInformation.DependencyName != null &&
                                                    registration.RegistrationId.Equals(typeInformation.DependencyName);
    }
}
