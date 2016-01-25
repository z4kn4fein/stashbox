using System;

namespace Stashbox.Attributes
{
    /// <summary>
    /// Represents an attribute for tracking constructor preferences.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class InjectionConstructorAttribute : Attribute
    { }
}