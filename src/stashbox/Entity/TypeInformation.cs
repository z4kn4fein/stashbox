using System;
using Stashbox.Attributes;

namespace Stashbox.Entity
{
    /// <summary>
    /// Represents type information about a dependency.
    /// </summary>
    public class TypeInformation
    {
        /// <summary>
        /// The reflected type of the dependency.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// The reflected type of the parent of the dependency.
        /// </summary>
        public Type ParentType { get; set; }

        /// <summary>
        /// The name of the dependency.
        /// </summary>
        public string DependencyName { get; set; }

        /// <summary>
        /// Custom attributes of the dependency.
        /// </summary>
        public Attribute[] CustomAttributes { get; set; }

        /// <summary>
        /// The variable name of the dependency.
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// It's true if the dependency has default value.
        /// </summary>
        public bool HasDefaultValue { get; set; }

        /// <summary>
        /// The default value of the dependency.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Property or field.
        /// </summary>
        public bool IsMember { get; set; }

        /// <summary>
        /// True if the type has <see cref="DependencyAttribute"/>.
        /// </summary>
        public bool HasDependencyAttribute { get; set; }

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        /// <param name="type">The type param to override.</param>
        /// <returns></returns>
        public TypeInformation Clone(Type type = null)
        {
            var clone = (TypeInformation)this.MemberwiseClone();
            if (type == null) return clone;

            clone.Type = type;
            return clone;
        }
    }
}