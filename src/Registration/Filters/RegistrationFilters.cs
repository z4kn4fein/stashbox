namespace Stashbox.Registration.Filters
{
    internal static class RegistrationFilters
    {
        public static IRegistrationFilter ConditionFilter = new ConditionFilter();
        public static IRegistrationFilter GenericFilter = new GenericFilter();
        public static IRegistrationFilter ScopeNameFilter = new ScopeNameFilter();
        public static IRegistrationFilter NameFilter = new NameFilter();
    }
}
