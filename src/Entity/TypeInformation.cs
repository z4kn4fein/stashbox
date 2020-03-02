using System;
using System.Collections.Generic;
using Stashbox.Utils;

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

        /// <summary>
        /// Property or field.
        /// </summary>
        public MemberType MemberType { get; internal set; }

        /// <summary>
        /// True if the dependency explicitly set to be injected.
        /// </summary>
        public bool ForcedDependency { get; internal set; }
        
        internal TypeInformation Clone(Type type)
        {
            var clone = this.Clone();
            clone.Type = type;
            return clone;
        }
        
        internal TypeInformation Clone()
        {
#if IL_EMIT
            return Cloner<TypeInformation>.Clone(this);
#else
            return (TypeInformation)this.MemberwiseClone();
#endif
        }
    }
}