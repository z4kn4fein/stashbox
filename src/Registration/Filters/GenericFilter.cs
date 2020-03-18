using System;
using Stashbox.Entity;
using Stashbox.Resolution;

namespace Stashbox.Registration.Filters
{
    internal class GenericFilter : IRegistrationFilter
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation,
            IServiceRegistration registration,
            ResolutionContext resolutionContext) => 
            !typeInformation.Type.IsClosedGenericType() || registration.ValidateGenericConstraints(typeInformation.Type);

        public bool ShouldIncrementWeight(TypeInformation typeInformation, IServiceRegistration registration,
            ResolutionContext resolutionContext) => false;
    }
}
