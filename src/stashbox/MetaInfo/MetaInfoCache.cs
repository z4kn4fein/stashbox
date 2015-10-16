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

        public MetaInfoCache(Type typeTo)
        {
            this.Constructors = new HashSet<ConstructorInformation>();
            this.TypeTo = typeTo;
            this.AddConstructors(typeTo.GetTypeInfo().DeclaredConstructors);
        }

        private void AddConstructors(IEnumerable<ConstructorInfo> infos)
        {
            foreach (var constructor in infos.Select(info => new ConstructorInformation
            {
                Method = info,
                HasInjectionAttribute = info.GetCustomAttribute<InjectionConstructorAttribute>() != null
            }))
            {
                constructor.Parameters.AddRange(this.FillParameters(constructor.Method.GetParameters()));

                this.Constructors.Add(constructor);
            }
        }

        private IEnumerable<ResolutionTarget> FillParameters(IEnumerable<ParameterInfo> parameters)
        {
            return parameters.Select(parameterInfo => new ResolutionTarget
            {
                TypeInformation = new TypeInformation
                {
                    Type = parameterInfo.ParameterType,
                    DependencyName = parameterInfo.GetCustomAttribute<DependencyAttribute>() != null ?
                                     parameterInfo.GetCustomAttribute<DependencyAttribute>().Name : null,
                    ParentType = this.TypeTo,
                    CustomAttributes = new HashSet<Attribute>(parameterInfo.GetCustomAttributes())
                },
                ResolutionTargetName = parameterInfo.Name
            });
        }
    }
}