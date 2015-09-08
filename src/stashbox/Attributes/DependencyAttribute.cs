using Ronin.Common;
using System;

namespace Stashbox.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DependencyAttribute : Attribute
    {
        public string Name { get; set; }

        public DependencyAttribute(string name)
        {
            Shield.EnsureNotNullOrEmpty(name);
            this.Name = name;
        }
    }
}