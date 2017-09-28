using System;
using System.Collections.Generic;

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
        public Type Type { get; internal set; }

        /// <summary>
        /// The reflected type of the parent of the dependency.
        /// </summary>
        public Type ParentType { get; internal set; }

        /// <summary>
        /// The name of the dependency.
        /// </summary>
        public object DependencyName { get; internal set; }

        /// <summary>
        /// Custom attributes of the dependency.
        /// </summary>
        public IEnumerable<Attribute> CustomAttributes { get; internal set; }

        /// <summary>
        /// The variable name of the dependency.
        /// </summary>
        public string ParameterName { get; internal set; }

        /// <summary>
        /// It's true if the dependency has default value.
        /// </summary>
        public bool HasDefaultValue { get; internal set; }

        /// <summary>
        /// The default value of the dependency.
        /// </summary>
        public object DefaultValue { get; internal set; }

        /// <summary>
        /// Property or field.
        /// </summary>
        public bool IsMember { get; internal set; }

        /// <summary>
        /// True if the dependency explicitly set to be injected.
        /// </summary>
        public bool ForcedDependency { get; internal set; }

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        /// <param name="type">The type param to override.</param>
        /// <returns></returns>
        public TypeInformation Clone(Type type)
        {
            var clone = (TypeInformation)this.MemberwiseClone();
            clone.Type = type;
            return clone;
        }
    }
}