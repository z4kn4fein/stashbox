using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stashbox.Attributes;
using Stashbox.Entity;

namespace Stashbox.MetaInfo
{
    /// <summary>
    /// Holds meta information about a service.
    /// </summary>
    public class MetaInformation
    {
        /// <summary>
        /// Holds the constructors of the service.
        /// </summary>
        public ConstructorInformation[] Constructors { get; private set; }

        /// <summary>
        /// Holds the injection methods of the service.
        /// </summary>
        public MethodInformation[] InjectionMethods { get; private set; }

        /// <summary>
        /// Holds the injection member of the service.
        /// </summary>
        public MemberInformation[] InjectionMembers { get; private set; }

        /// <summary>
        /// Holds the generic type constraints of the service.
        /// </summary>
        public IDictionary<int, Type[]> GenericTypeConstraints { get; }

        private readonly Type type;

        internal MetaInformation(Type typeTo)
        {
            this.type = typeTo;
            var typeInfo = typeTo.GetTypeInfo();
            this.GenericTypeConstraints = new Dictionary<int, Type[]>();
            this.AddConstructors(typeInfo.DeclaredConstructors);
            this.AddMethods(typeInfo.DeclaredMethods);
            this.InjectionMembers = this.FillMembers(typeInfo).CastToArray();
            this.CollectGenericConstraints(typeInfo);
        }

        /// <summary>
        /// Validates a type against the generic constraints of the service.
        /// </summary>
        /// <param name="typeForValidation">The validated type.</param>
        /// <returns>True if the given type is valid, otherwise false.</returns>
        public bool ValidateGenericContraints(Type typeForValidation)
        {
            var typeInfo = typeForValidation.GetTypeInfo();
            var length = typeInfo.GenericTypeArguments.Length;

            for (var i = 0; i < length; i++)
                if (this.GenericTypeConstraints.ContainsKey(i) && !this.GenericTypeConstraints[i].Contains(typeInfo.GenericTypeArguments[i]))
                    return false;

            return true;
        }

        /// <summary>
        /// Converts a <see cref="ParameterInfo"/> to <see cref="TypeInformation"/>.
        /// </summary>
        /// <param name="parameter">The parameter info.</param>
        /// <returns>The converted type info.</returns>
        public TypeInformation GetTypeInformationForParameter(ParameterInfo parameter) =>
            new TypeInformation
            {
                Type = parameter.ParameterType,
                DependencyName = parameter.GetCustomAttribute<DependencyAttribute>()?.Name,
                HasDependencyAttribute = parameter.GetCustomAttribute<DependencyAttribute>() != null,
                ParentType = this.type,
                CustomAttributes = parameter.GetCustomAttributes()?.ToArray(),
                ParameterName = parameter.Name,
                HasDefaultValue = parameter.HasDefaultValue(),
                DefaultValue = parameter.DefaultValue
            };


        private void AddConstructors(IEnumerable<ConstructorInfo> constructors) =>
            this.Constructors = constructors
            .Where(constructor => !constructor.IsStatic && constructor.IsPublic)
            .Select(info => new ConstructorInformation
            {
                Parameters = this.FillParameters(info.GetParameters()),
                Constructor = info
            }).CastToArray();


        private void AddMethods(IEnumerable<MethodInfo> infos) =>
            this.InjectionMethods = infos.Where(methodInfo => methodInfo.GetCustomAttribute<InjectionMethodAttribute>() != null).Select(info => new MethodInformation
            {
                Method = info,
                Parameters = this.FillParameters(info.GetParameters())
            }).CastToArray();

        private TypeInformation[] FillParameters(ParameterInfo[] parameters)
        {
            var length = parameters.Length;
            var types = new TypeInformation[length];

            for (int i = 0; i < length; i++)
                types[i] = this.GetTypeInformationForParameter(parameters[i]);


            return types;
        }

        private IEnumerable<MemberInformation> FillMembers(TypeInfo typeInfo)
        {
            return typeInfo.DeclaredProperties.Where(property => property.CanWrite && !property.IsIndexer())
                   .Select(propertyInfo => new MemberInformation
                   {
                       TypeInformation = new TypeInformation
                       {
                           Type = propertyInfo.PropertyType,
                           DependencyName = propertyInfo.GetCustomAttribute<DependencyAttribute>()?.Name,
                           HasDependencyAttribute = propertyInfo.GetCustomAttribute<DependencyAttribute>() != null,
                           ParentType = this.type,
                           CustomAttributes = propertyInfo.GetCustomAttributes()?.CastToArray(),
                           ParameterName = propertyInfo.Name,
                           IsMember = true
                       },
                       MemberInfo = propertyInfo
                   })
                   .Concat(typeInfo.DeclaredFields.Where(field => !field.IsInitOnly && !field.IsBackingField())
                           .Select(fieldInfo => new MemberInformation
                           {
                               TypeInformation = new TypeInformation
                               {
                                   Type = fieldInfo.FieldType,
                                   DependencyName = fieldInfo.GetCustomAttribute<DependencyAttribute>()?.Name,
                                   HasDependencyAttribute = fieldInfo.GetCustomAttribute<DependencyAttribute>() != null,
                                   ParentType = this.type,
                                   CustomAttributes = fieldInfo.GetCustomAttributes()?.CastToArray(),
                                   ParameterName = fieldInfo.Name,
                                   IsMember = true
                               },
                               MemberInfo = fieldInfo
                           }));
        }

        private void CollectGenericConstraints(TypeInfo typeInfo)
        {
            if (!typeInfo.IsGenericType && !typeInfo.IsGenericTypeDefinition)
                return;

            var length = typeInfo.GenericTypeParameters.Length;
            for (int i = 0; i < length; i++)
            {
                var typeInfoGenericTypeParameter = typeInfo.GenericTypeParameters[i];
                var paramTypeInfo = typeInfoGenericTypeParameter.GetTypeInfo();
                var cons = paramTypeInfo.GetGenericParameterConstraints();

                if (cons.Length > 0)
                {
                    var pos = paramTypeInfo.GenericParameterPosition;
                    this.GenericTypeConstraints.Add(pos, cons);
                }
            }
        }
    }
}
