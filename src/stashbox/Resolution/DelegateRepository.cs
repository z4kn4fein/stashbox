using System;
using System.Linq;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Utils;

namespace Stashbox.Resolution
{
    internal class DelegateRepository : IDelegateRepository
    {
        private readonly HashMap<Type, Func<object>> serviceDelegates;
        private readonly HashMap<string, Func<object>> keyedServiceDelegates;
        private readonly HashMap<Type, string, Delegate> factoryDelegates;

        public DelegateRepository()
        {
            this.serviceDelegates = new HashMap<Type, Func<object>>();
            this.keyedServiceDelegates = new HashMap<string, Func<object>>();
            this.factoryDelegates = new HashMap<Type, string, Delegate>();
        }
        
        public Delegate GetFactoryDelegateCacheOrDefault(TypeInformation typeInfo, Type[] parameterTypes)
        {
            var key = this.GetFactoryKey(parameterTypes, typeInfo.DependencyName);
            return this.factoryDelegates.GetOrDefault(typeInfo.Type, key);
        }

        public Func<object> GetDelegateCacheOrDefault(TypeInformation typeInfo)
        {
            return typeInfo.DependencyName == null ? this.serviceDelegates.GetOrDefault(typeInfo.Type)
                : this.keyedServiceDelegates.GetOrDefault(this.GetKey(typeInfo.Type, typeInfo.DependencyName));
        }
        
        public void AddFactoryDelegate(TypeInformation typeInfo, Type[] parameterTypes, Delegate factory)
        {
            var key = this.GetFactoryKey(parameterTypes, typeInfo.DependencyName);
            this.factoryDelegates.AddOrUpdate(typeInfo.Type, key, factory);
        }
        public void AddServiceDelegate(TypeInformation typeInfo, Func<object> factory)
        {
            if (typeInfo.DependencyName == null)
                this.serviceDelegates.AddOrUpdate(typeInfo.Type, factory);
            else
                this.keyedServiceDelegates.AddOrUpdate(this.GetKey(typeInfo.Type, typeInfo.DependencyName), factory);
        }

        public void InvalidateDelegateCache()
        {
            this.serviceDelegates.Clear();
            this.factoryDelegates.Clear();
            this.keyedServiceDelegates.Clear();
        }

        private string GetKey(Type type, string name) =>
            NameGenerator.GetRegistrationName(type, type, name);

        private string GetFactoryKey(Type[] types, string name) => types.Select(type => NameGenerator.GetRegistrationName(type, type))
            .Aggregate((current, next) => current + next) + name;
    }
}
