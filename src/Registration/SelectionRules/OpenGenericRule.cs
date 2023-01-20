using Stashbox.Resolution;
using System;

namespace Stashbox.Registration.SelectionRules;

internal class OpenGenericRule : IRegistrationSelectionRule
{
    public bool IsValidForCurrentRequest(TypeInformation typeInformation,
        ServiceRegistration registration, ResolutionContext resolutionContext, out bool shouldIncrementWeight)
    {
        shouldIncrementWeight = false;
        return !typeInformation.Type.IsClosedGenericType() ||
               registration.ImplementationType.SatisfiesGenericConstraintsOf(typeInformation.Type);
    }
}