using Stashbox.Registration.ServiceRegistrations;
using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules
{
    internal class NameRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation,
            ServiceRegistration registration, ResolutionContext resolutionContext, out bool shouldIncrementWeight)
        {
            if (typeInformation.DependencyName == null &&
                registration.Name == null)
            {
                shouldIncrementWeight = false;
                return true;
            }

            if (typeInformation.DependencyName != null &&
                registration.Name != null &&
                registration.Name.Equals(typeInformation.DependencyName))
            {
                shouldIncrementWeight = true;
                return true;
            }

            if (typeInformation.DependencyName == null &&
                registration.Name != null &&
                resolutionContext.CurrentContainerContext.ContainerConfiguration.NamedDependencyResolutionForUnNamedRequestsEnabled)
            {
                shouldIncrementWeight = false;
                return true;
            }

            if (typeInformation.DependencyName != null &&
                resolutionContext.CurrentContainerContext.ContainerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled &&
                (registration.Name == null || (registration.Name != null && registration.Name.Equals(typeInformation.DependencyName))))
            {
                shouldIncrementWeight = false;
                return true;
            }

            shouldIncrementWeight = false;
            return false;
        }
    }
}
