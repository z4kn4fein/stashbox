using Stashbox.Attributes;
using Stashbox.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.MetaInfo
{
    /// <summary>
    /// Represents a store which contains metadata about the services.
    /// </summary>
    public class MetaInfoCache
    {
        /// <summary>
        /// The type of the actual service implementation.
        /// </summary>
        public Type TypeTo { get; }

        /// <summary>
        /// Stores the reflected constructor informations.
        /// </summary>
        public ConstructorInformation[] Constructors { get; private set; }

        /// <summary>
        /// Stores the reflected injection method informations.
        /// </summary>
        public MethodInformation[] InjectionMethods { get; private set; }

        /// <summary>
        /// Stores the reflected injection memeber informations.
        /// </summary>
        public MemberInformation[] InjectionMembers { get; private set; }

        /// <summary>
        /// Constructs the <see cref="MetaInfoCache"/>
        /// </summary>
        /// <param name="typeTo">The type of the actual service implementation.</param>
        public MetaInfoCache(Type typeTo)
        {
            this.TypeTo = typeTo;

            var typeInfo = typeTo.GetTypeInfo();
            this.AddConstructors(typeInfo.DeclaredConstructors);
            this.AddMethods(typeInfo.DeclaredMethods);
            this.InjectionMembers = this.FillMembers(typeInfo).ToArray();
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

        private IEnumerable<MemberInformation> FillMembers(TypeInfo typeInfo)
        {
            return typeInfo.DeclaredProperties
                   .Where(propertyInfo => propertyInfo.GetCustomAttribute<DependencyAttribute>() != null)
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
                   })
                   .Concat(typeInfo.DeclaredFields
                           .Where(fieldInfo => fieldInfo.GetCustomAttribute<DependencyAttribute>() != null)
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
