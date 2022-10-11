using System;
using System.Collections.Generic;

namespace Stashbox.Resolution
{
    /// <summary>
    /// Represents type information about a dependency.
    /// </summary>
    public readonly struct TypeInformation
    {
        /// <summary>
        /// The reflected type of the dependency.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// The name of the dependency.
        /// </summary>
        public readonly object? DependencyName;

        /// <summary>
        /// The type of the metadata.
        /// </summary>
        public readonly Type? MetadataType;

        /// <summary>
        /// The reflected type of the parent of the dependency.
        /// </summary>
        public readonly Type? ParentType;

        /// <summary>
        /// Custom attributes of the dependency.
        /// </summary>
        public readonly IEnumerable<Attribute>? CustomAttributes;

        /// <summary>
        /// If the dependency is a method or a constructor parameter, this property holds the parameter name, if it's a class member then the member name.
        /// </summary>
        public readonly string? ParameterOrMemberName;

        /// <summary>
        /// It's true if the dependency has default value.
        /// </summary>
        public readonly bool HasDefaultValue;

        /// <summary>
        /// The default value of the dependency.
        /// </summary>
        public readonly object? DefaultValue;

        internal TypeInformation(Type type, object? dependencyName)
        {
            this.Type = type;
            this.DependencyName = dependencyName;
            this.ParentType = null;
            this.MetadataType = null;
            this.CustomAttributes = null;
            this.ParameterOrMemberName = null;
            this.HasDefaultValue = false;
            this.DefaultValue = null;
        }

        internal TypeInformation(Type type, Type? parentType, object? dependencyName,
            IEnumerable<Attribute>? customAttributes, string? parameterOrMemberName,
            bool hasDefaultValue, object? defaultValue, Type? metaDataType)
        {
            this.Type = type;
            this.ParentType = parentType;
            this.DependencyName = dependencyName;
            this.CustomAttributes = customAttributes;
            this.ParameterOrMemberName = parameterOrMemberName;
            this.HasDefaultValue = hasDefaultValue;
            this.DefaultValue = defaultValue;
            this.MetadataType = metaDataType;
        }

        /// <summary>
        /// Clones the type information with different type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="dependencyName">The dependency name.</param>
        /// <param name="metadataType">The metadata type.</param>
        /// <returns>The cloned type information.</returns>
        public TypeInformation Clone(Type type, object? dependencyName = null, Type? metadataType = null) =>
            new(type,
                this.ParentType,
                dependencyName ?? this.DependencyName,
                this.CustomAttributes,
                this.ParameterOrMemberName,
                this.HasDefaultValue,
                this.DefaultValue,
                metadataType ?? this.MetadataType);

        internal static readonly TypeInformation Empty = default;
    }
}