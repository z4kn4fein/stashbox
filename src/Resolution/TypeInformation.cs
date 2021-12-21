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
        public Type Type { get; private set; }

        /// <summary>
        /// The reflected type of the parent of the dependency.
        /// </summary>
        public Type ParentType { get; }

        /// <summary>
        /// The name of the dependency.
        /// </summary>
        public object DependencyName { get; }

        /// <summary>
        /// Custom attributes of the dependency.
        /// </summary>
        public IEnumerable<Attribute> CustomAttributes { get; }

        /// <summary>
        /// If the dependency is a method or a constructor parameter, this property holds the parameter name, if it's a class member then the member name.
        /// </summary>
        public string ParameterOrMemberName { get; }

        /// <summary>
        /// It's true if the dependency has default value.
        /// </summary>
        public bool HasDefaultValue { get; }

        /// <summary>
        /// The default value of the dependency.
        /// </summary>
        public object DefaultValue { get; }

        internal TypeInformation(Type type, object dependencyName)
        {
            this.Type = type;
            this.DependencyName = dependencyName;
        }

        internal TypeInformation(Type type, Type parentType, object dependencyName,
            IEnumerable<Attribute> customAttributes, string parameterOrMemberName,
            bool hasDefaultValue, object defaultValue)
        {
            this.Type = type;
            this.ParentType = parentType;
            this.DependencyName = dependencyName;
            this.CustomAttributes = customAttributes;
            this.ParameterOrMemberName = parameterOrMemberName;
            this.HasDefaultValue = hasDefaultValue;
            this.DefaultValue = defaultValue;
        }

        internal TypeInformation CloneForType(Type type)
        {
            var clone = (TypeInformation)this.MemberwiseClone();
            clone.Type = type;
            return clone;
        }
    }
}