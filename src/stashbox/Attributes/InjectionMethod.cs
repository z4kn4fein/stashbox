using System;

namespace Stashbox.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InjectionMethodAttribute : Attribute
    { }
}
