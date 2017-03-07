using System;
using System.Linq;
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
        
        public Delegate GetFactoryDelegateCacheOrDefault(Type type, Type[] parameterTypes, string name = null)
        {
            var key = this.GetFactoryKey(type, parameterTypes, name);
            return this.factoryDelegates.GetOrDefault(type, key);
        }

        public Func<object> GetDelegateCacheOrDefault(Type type, string name = null)
        {
            return name == null ? this.serviceDelegates.GetOrDefault(type)
                : this.keyedServiceDelegates.GetOrDefault(this.GetKey(type, name));
        }
        
        public void AddFactoryDelegate(Type type, Type[] parameterTypes, Delegate factory, string name = null)
        {
            var key = this.GetFactoryKey(type, parameterTypes, name);
            this.factoryDelegates.AddOrUpdate(type, key, factory);
        }
        public void AddServiceDelegate(Type type, Func<object> factory, string name = null)
        {
            if (name == null)
                this.serviceDelegates.AddOrUpdate(type, factory);
            else
                this.keyedServiceDelegates.AddOrUpdate(this.GetKey(type, name), factory);
        }

        public void InvalidateDelegateCache()
        {
            this.serviceDelegates.Clear();
            this.factoryDelegates.Clear();
            this.keyedServiceDelegates.Clear();
        }

        private string GetKey(Type type, string name) =>
            NameGenerator.GetRegistrationName(type, type, name);

        private string GetFactoryKey(Type returnType, Type[] types, string name)
        {
            name = this.GetKey(returnType, name);
            return types.Any() ? types.Select(type => NameGenerator.GetRegistrationName(type, type))
                .Aggregate((current, next) => current + next) + name : name;
        }
    }
}
