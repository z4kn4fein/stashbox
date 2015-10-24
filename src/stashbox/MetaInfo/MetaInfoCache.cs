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
        public HashSet<ConstructorInformation> Constructors { get; }
        public HashSet<MethodInformation> InjectionMethods { get; }
        public HashSet<PropertyInformation> InjectionProperties { get; }

        public MetaInfoCache(Type typeTo)
        {
            this.Constructors = new HashSet<ConstructorInformation>();
            this.InjectionMethods = new HashSet<MethodInformation>();
            this.TypeTo = typeTo;

            var typeInfo = typeTo.GetTypeInfo();
            this.AddConstructors(typeInfo.DeclaredConstructors);
            this.AddMethods(typeInfo.DeclaredMethods);
            this.InjectionProperties = new HashSet<PropertyInformation>(this.FillProperties(typeInfo.DeclaredProperties));
        }

        private void AddConstructors(IEnumerable<ConstructorInfo> infos)
        {
            foreach (var constructor in infos.Select(info => new ConstructorInformation
            {
                Constructor = info,
                HasInjectionAttribute = info.GetCustomAttribute<InjectionConstructorAttribute>() != null
            }))
            {
                constructor.Parameters = new HashSet<TypeInformation>(this.FillParameters(constructor.Constructor.GetParameters()));
                this.Constructors.Add(constructor);
            }
        }

        private void AddMethods(IEnumerable<MethodInfo> infos)
        {
            foreach (var method in infos.Where(methodInfo => methodInfo.GetCustomAttribute<InjectionMethodAttribute>() != null).Select(info => new MethodInformation
            {
                Method = info
            }))
            {
                method.Parameters = new HashSet<TypeInformation>(this.FillParameters(method.Method.GetParameters()));
                this.InjectionMethods.Add(method);
            }
        }

        private IEnumerable<TypeInformation> FillParameters(IEnumerable<ParameterInfo> parameters)
        {
            return parameters.Select(parameterInfo => new TypeInformation
            {
                Type = parameterInfo.ParameterType,
                DependencyName = parameterInfo.GetCustomAttribute<DependencyAttribute>() != null ?
                                 parameterInfo.GetCustomAttribute<DependencyAttribute>().Name : null,
                ParentType = this.TypeTo,
                CustomAttributes = new HashSet<Attribute>(parameterInfo.GetCustomAttributes()),
                MemberName = parameterInfo.Name
            });
        }

        private IEnumerable<PropertyInformation> FillProperties(IEnumerable<PropertyInfo> properties)
        {
            return properties.Where(propertyInfo => propertyInfo.GetCustomAttribute<DependencyAttribute>() != null)
                             .Select(propertyInfo => new PropertyInformation
                             {
                                 TypeInformation = new TypeInformation
                                 {
                                     Type = propertyInfo.PropertyType,
                                     DependencyName = propertyInfo.GetCustomAttribute<DependencyAttribute>() != null ?
                                                  propertyInfo.GetCustomAttribute<DependencyAttribute>().Name : null,
                                     ParentType = this.TypeTo,
                                     CustomAttributes = new HashSet<Attribute>(propertyInfo.GetCustomAttributes()),
                                     MemberName = propertyInfo.Name
                                 },
                                 PropertyInfo = propertyInfo
                             });
        }
    }
}
