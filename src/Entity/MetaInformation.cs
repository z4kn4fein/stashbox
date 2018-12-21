using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.Entity
{
    /// <summary>
    /// Holds meta information about a service.
    /// </summary>
    internal class MetaInformation
    {
        private readonly IDictionary<int, GenericConstraintInfo> genericTypeConstraints;
        private readonly Type type;

        /// <summary>
        /// Holds the constructors of the service.
        /// </summary>
        public ConstructorInformation[] Constructors { get; }

        /// <summary>
        /// Holds the injection methods of the service.
        /// </summary>
        public MethodInformation[] InjectionMethods { get; }

        /// <summary>
        /// Holds the injection member of the service.
        /// </summary>
        public MemberInformation[] InjectionMembers { get; }

        /// <summary>
        /// Returns true if the underlying type is open generic, otherwise false.
        /// </summary>
        public bool IsOpenGenericType { get; }

        internal MetaInformation(Type typeTo)
        {
            this.type = typeTo;
            var typeInfo = this.type.GetTypeInfo();
            this.IsOpenGenericType = typeInfo.IsOpenGenericType();
            this.genericTypeConstraints = new Dictionary<int, GenericConstraintInfo>();
            this.Constructors = this.CollectConstructors(typeInfo.DeclaredConstructors);
            this.InjectionMethods = this.CollectMethods(typeInfo.DeclaredMethods);
            this.InjectionMembers = this.CollectMembers(typeInfo);
            this.CollectGenericConstraints(typeInfo);
        }

        /// <summary>
        /// Validates a type against the generic constraints of the service.
        /// </summary>
        /// <param name="typeForValidation">The validated type.</param>
        /// <returns>True if the given type is valid, otherwise false.</returns>
        public bool ValidateGenericContraints(Type typeForValidation)
        {
            if (this.genericTypeConstraints.Count == 0)
                return true;

            var typeInfo = typeForValidation.GetTypeInfo();
            var length = typeInfo.GenericTypeArguments.Length;

            for (var i = 0; i < length; i++)
            {
                var genericArgument = typeInfo.GenericTypeArguments[i];
                var genericArgumentInfo = genericArgument.GetTypeInfo();
                if (this.genericTypeConstraints.TryGetValue(i, out var constraint))
                {
                    if ((constraint.GenericParameterConstraints & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint &&
                        !genericArgumentInfo.IsPrimitive &&
                        !genericArgumentInfo.HasPublicParameterlessConstructor())
                        return false;

                    if ((constraint.GenericParameterConstraints & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint &&
                        !genericArgumentInfo.IsClass)
                        return false;

                    if (constraint.TypeConstraints.Length > 0 && !constraint.TypeConstraints.Any(c =>
                    {
                        var con = c.IsClosedGenericType() ? c.GetGenericTypeDefinition().MakeGenericType(genericArgument) : c;
                        return genericArgumentInfo.Implements(con);
                    }))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Converts a <see cref="ParameterInfo"/> to <see cref="TypeInformation"/>.
        /// </summary>
        /// <param name="parameter">The parameter info.</param>
        /// <returns>The converted type info.</returns>
        public TypeInformation GetTypeInformationForParameter(ParameterInfo parameter)
        {
            var customAttributes = parameter.GetCustomAttributes();
            var dependencyAttribute = parameter.GetDependencyAttribute();
            return new TypeInformation
            {
                Type = parameter.ParameterType,
                DependencyName = dependencyAttribute?.Name,
                ForcedDependency = dependencyAttribute != null,
                ParentType = this.type,
                CustomAttributes = customAttributes,
                ParameterName = parameter.Name,
                HasDefaultValue = parameter.HasDefaultValue(),
                DefaultValue = parameter.DefaultValue
            };
        }

        private ConstructorInformation[] CollectConstructors(IEnumerable<ConstructorInfo> constructors) =>
            constructors.Where(constructor => !constructor.IsStatic && constructor.IsPublic)
            .Select(info => new ConstructorInformation
            {
                Parameters = this.CollectParameters(info.GetParameters()),
                Constructor = info
            }).CastToArray();


        private MethodInformation[] CollectMethods(IEnumerable<MethodInfo> infos) =>
            infos.Where(methodInfo => methodInfo.GetInjectionAttribute() != null).Select(info => new MethodInformation
            {
                Method = info,
                Parameters = this.CollectParameters(info.GetParameters())
            }).CastToArray();

        private TypeInformation[] CollectParameters(ParameterInfo[] parameters)
        {
            var length = parameters.Length;
            var types = new TypeInformation[length];

            for (var i = 0; i < length; i++)
                types[i] = this.GetTypeInformationForParameter(parameters[i]);

            return types;
        }

        private MemberInformation[] CollectMembers(TypeInfo typeInfo)
        {
            var members = this.CollectProperties(typeInfo)
                   .Concat(this.CollectFields(typeInfo));

            var baseType = typeInfo.BaseType;
            while (baseType != null && !baseType.IsObjectType())
            {
                var baseTypeInfo = baseType.GetTypeInfo();
                members = members.Concat(this.CollectProperties(baseTypeInfo)
                    .Concat(this.CollectFields(baseTypeInfo)));
                baseType = baseTypeInfo.BaseType;
            }

            return members.CastToArray();
        }

        private IEnumerable<MemberInformation> CollectProperties(TypeInfo typeInfo) =>
           typeInfo.DeclaredProperties.Where(property => property.CanWrite && !property.IsIndexer())
                .Select(propertyInfo =>
                {
                    var attr = propertyInfo.GetDependencyAttribute();
                    return new MemberInformation
                    {
                        TypeInformation = new TypeInformation
                        {
                            Type = propertyInfo.PropertyType,
                            DependencyName = attr?.Name,
                            ForcedDependency = attr != null,
                            ParentType = this.type,
                            CustomAttributes = propertyInfo.GetCustomAttributes()?.CastToArray(),
                            ParameterName = propertyInfo.Name,
                            MemberType = MemberType.Property
                        },
                        MemberInfo = propertyInfo
                    };
                });

        private IEnumerable<MemberInformation> CollectFields(TypeInfo typeInfo) =>
            typeInfo.DeclaredFields.Where(field => !field.IsInitOnly && !field.IsBackingField())
                .Select(fieldInfo =>
                {
                    var attr = fieldInfo.GetDependencyAttribute();
                    return new MemberInformation
                    {
                        TypeInformation = new TypeInformation
                        {
                            Type = fieldInfo.FieldType,
                            DependencyName = attr?.Name,
                            ForcedDependency = attr != null,
                            ParentType = this.type,
                            CustomAttributes = fieldInfo.GetCustomAttributes()?.CastToArray(),
                            ParameterName = fieldInfo.Name,
                            MemberType = MemberType.Field
                        },
                        MemberInfo = fieldInfo
                    };
                });

        private void CollectGenericConstraints(TypeInfo typeInfo)
        {
            if (!typeInfo.IsGenericType && !typeInfo.IsGenericTypeDefinition)
                return;

            var length = typeInfo.GenericTypeParameters.Length;
            for (var i = 0; i < length; i++)
            {
                var typeInfoGenericTypeParameter = typeInfo.GenericTypeParameters[i];
                var paramTypeInfo = typeInfoGenericTypeParameter.GetTypeInfo();
                var cons = paramTypeInfo.GetGenericParameterConstraints();
                var attributes = paramTypeInfo.GenericParameterAttributes;

                if (cons.Length > 0 || (attributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint ||
                    (attributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint)
                {
                    var pos = paramTypeInfo.GenericParameterPosition;
                    this.genericTypeConstraints.Add(pos, new GenericConstraintInfo { TypeConstraints = cons, GenericParameterConstraints = attributes });
                }
            }
        }
    }
}
