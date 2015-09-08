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

            this.AddConstructors(typeTo.GetTypeInfo().DeclaredConstructors);
            this.TypeTo = typeTo;
        }

        private void AddConstructors(IEnumerable<ConstructorInfo> infos)
        {
            foreach (var info in infos)
            {
                var constructor = new ConstructorInformation
                {
                    Method = info,
                    HasInjectionAttribute = info.GetCustomAttribute<InjectionConstructorAttribute>() != null
                };

                constructor.Parameters.AddRange(this.FillParameters(constructor.Method.GetParameters()));

                this.Constructors.Add(constructor);
            }
        }

        private IEnumerable<ParameterInformation> FillParameters(IEnumerable<ParameterInfo> parameters)
        {
            return parameters.Select(x => new ParameterInformation
            {
                Type = x.ParameterType,
                ParameterInfo = x,
                DependencyName = x.GetCustomAttribute<DependencyAttribute>() != null ?
                    x.GetCustomAttribute<DependencyAttribute>().Name : null
            });
        }
    }
}