using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stashbox.Attributes;
using Stashbox.Entity;

namespace Stashbox.MetaInfo
{
    internal class MetaInformation
    {
        public MethodInformation[] InjectionMethods { get; private set; }
        public MemberInformation[] InjectionMembers { get; private set; }
        public ConstructorInformation[] Constructors { get; private set; }
        public IDictionary<int, Type[]> GenericTypeConstraints { get; }

        private readonly Type typeTo;

        public MetaInformation(Type typeTo)
        {
            this.typeTo = typeTo;
            this.GenericTypeConstraints = new Dictionary<int, Type[]>();
            var typeInfo = typeTo.GetTypeInfo();
            this.AddConstructors(typeInfo.DeclaredConstructors);
            this.AddMethods(typeInfo.DeclaredMethods);
            this.InjectionMembers = this.FillMembers(typeInfo).CastToArray();
            this.CollectGenericConstraints(typeInfo);
        }

        private void AddConstructors(IEnumerable<ConstructorInfo> infos)
        {
            this.Constructors = infos.Where(info => !info.IsStatic).Select(info => new ConstructorInformation
            {
                Constructor = info,
                Parameters = this.FillParameters(info.GetParameters()).CastToArray()
            }).CastToArray();
        }

        private void AddMethods(IEnumerable<MethodInfo> infos)
        {
            this.InjectionMethods = infos.Where(methodInfo => methodInfo.GetCustomAttribute<InjectionMethodAttribute>() != null).Select(info => new MethodInformation
            {
                Method = info,
                Parameters = this.FillParameters(info.GetParameters()).CastToArray()
            }).CastToArray();
        }

        private IEnumerable<TypeInformation> FillParameters(IEnumerable<ParameterInfo> parameters)
        {
            return parameters.Select(parameterInfo => new TypeInformation
            {
                Type = parameterInfo.ParameterType,
                DependencyName = parameterInfo.GetCustomAttribute<DependencyAttribute>()?.Name,
                HasDependencyAttribute = parameterInfo.GetCustomAttribute<DependencyAttribute>() != null,
                ParentType = this.typeTo,
                CustomAttributes = parameterInfo.GetCustomAttributes()?.ToArray(),
                ParameterName = parameterInfo.Name,
                HasDefaultValue = parameterInfo.HasDefaultValue(),
                DefaultValue = parameterInfo.DefaultValue
            });
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
                           ParentType = this.typeTo,
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
                                   ParentType = this.typeTo,
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

            foreach (var typeInfoGenericTypeParameter in typeInfo.GenericTypeParameters)
            {
                var paramTypeInfo = typeInfoGenericTypeParameter.GetTypeInfo();
                var pos = paramTypeInfo.GenericParameterPosition;
                var cons = paramTypeInfo.GetGenericParameterConstraints();

                if (cons.Length > 0)
                    this.GenericTypeConstraints.Add(pos, cons);
            }
        }
    }
}
