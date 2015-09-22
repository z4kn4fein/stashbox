using System;

namespace Stashbox.ContainerExtensions.MethodInjection
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InjectionMethodAttribute : Attribute
    { }
}
