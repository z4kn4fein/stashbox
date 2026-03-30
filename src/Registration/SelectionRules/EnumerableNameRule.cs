using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules;

internal class EnumerableNameRule : IRegistrationSelectionRule
{
    public bool IsValidForCurrentRequest(TypeInformation typeInformation,
        ServiceRegistration registration, ResolutionContext resolutionContext, out bool shouldIncrementWeight)
    {
        if (typeInformation.DependencyName == null)
        {
            shouldIncrementWeight = false;
            return resolutionContext.CurrentContainerContext.ContainerConfiguration.NamedDependencyResolutionForUnNamedCollectionRequestsEnabled || registration.Name == null;
        }

        if (resolutionContext.CurrentContainerContext.ContainerConfiguration.IgnoreServicesWithUniversalNameForUniversalNamedRequests &&
            typeInformation.DependencyName != null && typeInformation.DependencyName.Equals(resolutionContext.CurrentContainerContext.ContainerConfiguration.UniversalName) &&
            registration.Name != null && registration.Name.Equals(resolutionContext.CurrentContainerContext.ContainerConfiguration.UniversalName))
        {
            shouldIncrementWeight = false;
            return false;
        }
        
        if (typeInformation.DependencyName != null &&
            registration.Name != null &&
            registration.Name.Equals(typeInformation.DependencyName))
        {
            shouldIncrementWeight = true;
            return true;
        }
        
        if (typeInformation.DependencyName != null &&
            typeInformation.DependencyName.Equals(resolutionContext.CurrentContainerContext.ContainerConfiguration.UniversalName) &&
            registration.Name != null)
        {
            shouldIncrementWeight = true;
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