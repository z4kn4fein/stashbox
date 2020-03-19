namespace Stashbox.Registration.SelectionRules
{
    internal static class RegistrationSelectionRules
    {
        public static IRegistrationSelectionRule ConditionFilter = new ConditionRule();
        public static IRegistrationSelectionRule GenericFilter = new GenericRule();
        public static IRegistrationSelectionRule ScopeNameFilter = new ScopeNameRule();
        public static IRegistrationSelectionRule NameFilter = new NameRule();
    }
}
