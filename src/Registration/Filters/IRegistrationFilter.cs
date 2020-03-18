using Stashbox.Entity;
using Stashbox.Resolution;

namespace Stashbox.Registration.Filters
{
    internal interface IRegistrationFilter
    {
        bool IsValidForCurrentRequest(TypeInformation typeInformation,
            IServiceRegistration registration, ResolutionContext resolutionContext);

        bool ShouldIncrementWeight(TypeInformation typeInformation,
            IServiceRegistration registration, ResolutionContext resolutionContext);
    }
}
