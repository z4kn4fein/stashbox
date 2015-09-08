using System;

namespace Stashbox.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class InjectionPropertyAttribute : Attribute
    {
        public string Name { get; set; }

        public InjectionPropertyAttribute(string name = null)
        {
            this.Name = name;
        }
    }
}