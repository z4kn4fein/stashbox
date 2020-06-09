using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules
{
    internal class NameRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation, ServiceRegistration registration,
            ResolutionContext resolutionContext)
        {
            if (typeInformation.DependencyName != null &&
                registration.RegistrationContext.Name != null &&
                !registration.RegistrationContext.Name.Equals(typeInformation.DependencyName) &&
                !registration.IsResolvableByUnnamedRequest)
                return false;

            return typeInformation.DependencyName != null || registration.IsResolvableByUnnamedRequest;
        }

        public bool ShouldIncrementWeight(TypeInformation typeInformation, ServiceRegistration registration,
            ResolutionContext resolutionContext) => typeInformation.DependencyName != null &&
                                                    registration.RegistrationContext.Name != null &&
                                                    registration.RegistrationContext.Name.Equals(typeInformation.DependencyName);
    }
}
