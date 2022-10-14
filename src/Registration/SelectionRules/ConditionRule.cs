using Stashbox.Resolution;
using System.Collections.Generic;

namespace Stashbox.Registration.SelectionRules
{
    internal class ConditionRule : IRegistrationSelectionRule
    {
        public bool IsValidForCurrentRequest(TypeInformation typeInformation,
            ServiceRegistration registration, ResolutionContext resolutionContext, out bool shouldIncrementWeight)
        {
            var conditions = registration.Options.GetOrDefault<ConditionOptions>(RegistrationOption.ConditionOptions);
            if (conditions is not null)
            {
                shouldIncrementWeight = registration.IsUsableForCurrentContext(typeInformation, conditions);
                return shouldIncrementWeight;
            }

            shouldIncrementWeight = false;
            return conditions is null;
        }
    }
}
