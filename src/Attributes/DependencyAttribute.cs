using System;

namespace Stashbox.Attributes
{
    /// <summary>
    /// Represents an attribute for tracking dependencies.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field)]
    public class DependencyAttribute : Attribute
    {
        /// <summary>
        /// The name of the dependency.
        /// </summary>
        public object Name { get; set; }

        /// <summary>
        /// Constructs a <see cref="DependencyAttribute"/>
        /// </summary>
        /// <param name="name">The name of the dependency.</param>
        public DependencyAttribute(object name = null)
        {
            this.Name = name;
        }
    }
}