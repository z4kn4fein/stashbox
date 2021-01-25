namespace Stashbox.Registration.SelectionRules
{
    internal static class RegistrationSelectionRules
    {
        public static readonly IRegistrationSelectionRule ConditionFilter = new ConditionRule();
        public static readonly IRegistrationSelectionRule GenericFilter = new OpenGenericRule();
        public static readonly IRegistrationSelectionRule ScopeNameFilter = new ScopeNameRule();
        public static readonly IRegistrationSelectionRule NameFilter = new NameRule();
    }
}
