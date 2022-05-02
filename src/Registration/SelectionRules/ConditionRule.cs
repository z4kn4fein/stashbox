using Stashbox.Registration.ServiceRegistrations;
using Stashbox.Resolution;

namespace Stashbox.Registration.SelectionRules
{
    internal class ConditionRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation,
            ServiceRegistration registration, ResolutionContext resolutionContext, out bool shouldIncrementWeight)
        {
            if (registration is not ComplexRegistration complexRegistration)
            {
                shouldIncrementWeight = false;
                return true;
            }

            if (HasCondition(complexRegistration))
            {
                shouldIncrementWeight = complexRegistration.IsUsableForCurrentContext(typeInformation);
                return shouldIncrementWeight;
            }

            shouldIncrementWeight = false;
            return !HasCondition(complexRegistration);
        }

        private static bool HasCondition(ComplexRegistration serviceRegistration) =>
            serviceRegistration.TargetTypeConditions != null ||
            serviceRegistration.ResolutionConditions != null ||
            serviceRegistration.AttributeConditions != null;
    }
}
