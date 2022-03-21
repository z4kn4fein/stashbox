using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules
{
    internal class NameRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation,
            ServiceRegistration registration, ResolutionContext resolutionContext)
        {
            if (typeInformation.DependencyName != null &&
                registration.Name != null &&
                !registration.Name.Equals(typeInformation.DependencyName) &&
                !resolutionContext.CurrentContainerContext
                    .ContainerConfiguration.NamedDependencyResolutionForUnNamedRequestsEnabled)
                return false;

            return typeInformation.DependencyName != null ||
                registration.Name == null ||
                resolutionContext.CurrentContainerContext.ContainerConfiguration.NamedDependencyResolutionForUnNamedRequestsEnabled;
        }

        public bool ShouldIncrementWeight(TypeInformation typeInformation,
            ServiceRegistration registration, ResolutionContext resolutionContext) =>
            typeInformation.DependencyName != null &&
            registration.Name != null &&
            registration.Name.Equals(typeInformation.DependencyName);
    }
}
