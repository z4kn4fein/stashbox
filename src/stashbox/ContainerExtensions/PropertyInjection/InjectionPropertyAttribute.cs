using System;

namespace Stashbox.ContainerExtensions.PropertyInjection
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InjectionPropertyAttribute : Attribute
    {
        public string Name { get; set; }

        public InjectionPropertyAttribute(string name = null)
        {
            this.Name = name;
        }
    }
}
