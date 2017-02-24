using Stashbox.Attributes;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stashbox.Configuration;
using Stashbox.Registration;

namespace Stashbox.MetaInfo
{
    internal class MetaInfoCache
    {
        private readonly bool autoMemberInjectionEnabled;
        private readonly Rules.AutoMemberInjection autoMemberInjectionRule;

        public readonly IDictionary<int, Type[]> GenericTypeConstraints;
        public Type TypeTo { get; }
        public ConstructorInformation[] Constructors { get; private set; }
        public MethodInformation[] InjectionMethods { get; private set; }
        public MemberInformation[] InjectionMembers { get; private set; }
        
        public MetaInfoCache(IContainerConfigurator containerConfigurator, RegistrationContextData registrationData, Type typeTo)
        {
            this.TypeTo = typeTo;
            this.GenericTypeConstraints = new Dictionary<int, Type[]>();

            this.autoMemberInjectionEnabled = containerConfigurator.ContainerConfiguration.MemberInjectionWithoutAnnotationEnabled || registrationData.AutoMemberInjectionEnabled;
            this.autoMemberInjectionRule = registrationData.AutoMemberInjectionEnabled ? registrationData.AutoMemberInjectionRule :
                containerConfigurator.ContainerConfiguration.MemberInjectionWithoutAnnotationRule;

            var typeInfo = typeTo.GetTypeInfo();
            this.AddConstructors(typeInfo.DeclaredConstructors);
            this.AddMethods(typeInfo.DeclaredMethods);
            this.InjectionMembers = this.FillMembers(typeInfo).ToArray();
            this.CollectGenericConstraints(typeInfo);
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

        private void AddConstructors(IEnumerable<ConstructorInfo> infos)
        {
            this.Constructors = infos.Where(info => !info.IsStatic).Select(info => new ConstructorInformation
            {
                Constructor = info,
                Parameters = this.FillParameters(info.GetParameters()).ToArray()
            }).ToArray();
        }

        private void AddMethods(IEnumerable<MethodInfo> infos)
        {
            this.InjectionMethods = infos.Where(methodInfo => methodInfo.GetCustomAttribute<InjectionMethodAttribute>() != null).Select(info => new MethodInformation
            {
                Method = info,
                Parameters = this.FillParameters(info.GetParameters()).ToArray()
            }).ToArray();
        }

        private IEnumerable<TypeInformation> FillParameters(IEnumerable<ParameterInfo> parameters)
        {
            return parameters.Select(parameterInfo => new TypeInformation
            {
                Type = parameterInfo.ParameterType,
                DependencyName = parameterInfo.GetCustomAttribute<DependencyAttribute>()?.Name,
                ParentType = this.TypeTo,
                CustomAttributes = parameterInfo.GetCustomAttributes().ToArray(),
                ParameterName = parameterInfo.Name,
                HasDefaultValue = parameterInfo.HasDefaultValue(),
                DefaultValue = parameterInfo.DefaultValue
            });
        }

        private IEnumerable<MemberInformation> FillMembers(TypeInfo typeInfo)
        {
            return this.SelectProperties(typeInfo.DeclaredProperties.Where(property => property.CanWrite && !property.IsIndexer()))
                   .Select(propertyInfo => new MemberInformation
                   {
                       TypeInformation = new TypeInformation
                       {
                           Type = propertyInfo.PropertyType,
                           DependencyName = propertyInfo.GetCustomAttribute<DependencyAttribute>()?.Name,
                           ParentType = this.TypeTo,
                           CustomAttributes = propertyInfo.GetCustomAttributes().ToArray(),
                           ParameterName = propertyInfo.Name,
                           IsMember = true
                       },
                       MemberInfo = propertyInfo
                   })
                   .Concat(this.SelectFields(typeInfo.DeclaredFields.Where(field => !field.IsInitOnly && !field.IsBackingField()))
                           .Select(fieldInfo => new MemberInformation
                           {
                               TypeInformation = new TypeInformation
                               {
                                   Type = fieldInfo.FieldType,
                                   DependencyName = fieldInfo.GetCustomAttribute<DependencyAttribute>()?.Name,
                                   ParentType = this.TypeTo,
                                   CustomAttributes = fieldInfo.GetCustomAttributes().ToArray(),
                                   ParameterName = fieldInfo.Name,
                                   IsMember = true
                               },
                               MemberInfo = fieldInfo
                           }));
        }

        private IEnumerable<FieldInfo> SelectFields(IEnumerable<FieldInfo> fields)
        {
            if (this.autoMemberInjectionEnabled)
                return fields.Where(fieldInfo => fieldInfo.GetCustomAttribute<DependencyAttribute>() != null ||
                    this.autoMemberInjectionRule.HasFlag(Rules.AutoMemberInjection.PrivateFields));
            else
                return fields.Where(fieldInfo => fieldInfo.GetCustomAttribute<DependencyAttribute>() != null);
        }

        private IEnumerable<PropertyInfo> SelectProperties(IEnumerable<PropertyInfo> properties)
        {
            if (this.autoMemberInjectionEnabled)
                return properties.Where(property => property.GetCustomAttribute<DependencyAttribute>() != null ||
                    (this.autoMemberInjectionRule.HasFlag(Rules.AutoMemberInjection.PropertiesWithPublicSetter) && property.HasSetMethod()) ||
                     this.autoMemberInjectionRule.HasFlag(Rules.AutoMemberInjection.PropertiesWithLimitedAccess));
            else
                return properties.Where(property => property.GetCustomAttribute<DependencyAttribute>() != null);
        }
    }
}
