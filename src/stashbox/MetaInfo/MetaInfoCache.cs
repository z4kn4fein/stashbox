using Stashbox.Attributes;
using Stashbox.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.MetaInfo
{
    public class MetaInfoCache
    {
        public Type TypeTo { get; }
        public ConstructorInformation[] Constructors { get; private set; }
        public MethodInformation[] InjectionMethods { get; private set; }
        public MemberInformation[] InjectionMembers { get; private set; }

        public MetaInfoCache(Type typeTo)
        {
            this.TypeTo = typeTo;

            var typeInfo = typeTo.GetTypeInfo();
            this.AddConstructors(typeInfo.DeclaredConstructors);
            this.AddMethods(typeInfo.DeclaredMethods);
            this.InjectionMembers = this.FillProperties(typeInfo).ToArray();
        }

        private void AddConstructors(IEnumerable<ConstructorInfo> infos)
        {
            this.Constructors = infos.Select(info => new ConstructorInformation
            {
                Constructor = info,
                HasInjectionAttribute = info.GetCustomAttribute<InjectionConstructorAttribute>() != null,
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
                DependencyName = parameterInfo.GetCustomAttribute<DependencyAttribute>() != null ?
                                 parameterInfo.GetCustomAttribute<DependencyAttribute>().Name : null,
                ParentType = this.TypeTo,
                CustomAttributes = parameterInfo.GetCustomAttributes().ToArray(),
                MemberName = parameterInfo.Name
            });
        }

        private IEnumerable<MemberInformation> FillProperties(TypeInfo typeInfo)
        {
            var properties =  typeInfo.DeclaredProperties.Where(propertyInfo => propertyInfo.GetCustomAttribute<DependencyAttribute>() != null)
                             .Select(propertyInfo => new MemberInformation
                             {
                                 TypeInformation = new TypeInformation
                                 {
                                     Type = propertyInfo.PropertyType,
                                     DependencyName = propertyInfo.GetCustomAttribute<DependencyAttribute>() != null ?
                                                  propertyInfo.GetCustomAttribute<DependencyAttribute>().Name : null,
                                     ParentType = this.TypeTo,
                                     CustomAttributes = propertyInfo.GetCustomAttributes().ToArray(),
                                     MemberName = propertyInfo.Name
                                 },
                                 MemberInfo = propertyInfo
                             });

            return properties.Concat(typeInfo.DeclaredFields.Where(
                fieldInfo => fieldInfo.GetCustomAttribute<DependencyAttribute>() != null)
                .Select(fieldInfo => new MemberInformation
                {
                    TypeInformation = new TypeInformation
                    {
                        Type = fieldInfo.FieldType,
                        DependencyName = fieldInfo.GetCustomAttribute<DependencyAttribute>() != null
                            ? fieldInfo.GetCustomAttribute<DependencyAttribute>().Name
                            : null,
                        ParentType = this.TypeTo,
                        CustomAttributes = fieldInfo.GetCustomAttributes().ToArray(),
                        MemberName = fieldInfo.Name
                    },
                    MemberInfo = fieldInfo
                }));
        }
    }
}
