﻿namespace Stashbox.Registration.SelectionRules;

internal static class RegistrationSelectionRules
{
    public static readonly IRegistrationSelectionRule ConditionFilter = new ConditionRule();
    public static readonly IRegistrationSelectionRule GenericFilter = new OpenGenericRule();
    public static readonly IRegistrationSelectionRule ScopeNameFilter = new ScopeNameRule();
    public static readonly IRegistrationSelectionRule NameFilter = new NameRule();
    public static readonly IRegistrationSelectionRule EnumerableNameFilter = new EnumerableNameRule();
    public static readonly IRegistrationSelectionRule MetadataFilter = new MetadataRule();
    public static readonly IRegistrationSelectionRule DecoratorFilter = new DecoratorRule();
}