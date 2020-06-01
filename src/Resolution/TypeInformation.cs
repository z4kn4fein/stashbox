using System;
using System.Collections.Generic;

namespace Stashbox.Resolution
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
        /// If the dependency is a method or constructor parameter this property holds the parameter name, if it's a class member then the member name.
        /// </summary>
        public string ParameterOrMemberName { get; internal set; }

        /// <summary>
        /// It's true if the dependency has default value.
        /// </summary>
        public bool HasDefaultValue { get; internal set; }

        /// <summary>
        /// The default value of the dependency.
        /// </summary>
        public object DefaultValue { get; internal set; }

        internal TypeInformation CloneForType(Type type)
        {
            var clone = (TypeInformation)this.MemberwiseClone();
            clone.Type = type;
            return clone;
        }
    }
}