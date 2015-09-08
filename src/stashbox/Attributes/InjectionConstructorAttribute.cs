using System;

namespace Stashbox.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class InjectionConstructorAttribute : Attribute
    { }
}