using System;
using Stashbox.Infrastructure;
using Stashbox.Utils;

namespace Stashbox.Resolution
{
    internal class DelegateRepository : IDelegateRepository
    {
        private readonly HashMap<Type, Func<IResolutionScope, object>> serviceDelegates;
        private readonly HashMap<object, Func<IResolutionScope, object>> keyedServiceDelegates;
        private readonly HashMap<Type, Func<IResolutionScope, Delegate>> factoryDelegates;
        private readonly HashMap<object, Func<IResolutionScope, Delegate>> keyedFactoryDelegates;

        public DelegateRepository()
        {
            this.serviceDelegates = new HashMap<Type, Func<IResolutionScope, object>>();
            this.keyedServiceDelegates = new HashMap<object, Func<IResolutionScope, object>>();
            this.factoryDelegates = new HashMap<Type, Func<IResolutionScope, Delegate>>();
            this.keyedFactoryDelegates = new HashMap<object, Func<IResolutionScope, Delegate>>();
        }

        public Func<IResolutionScope, Delegate> GetFactoryDelegateCacheOrDefault(Type type, object name = null)
        {
            return name == null ? this.factoryDelegates.GetOrDefault(type)
                : this.keyedFactoryDelegates.GetOrDefault(this.GetKey(type, name));
        }

        public Func<IResolutionScope, object> GetDelegateCacheOrDefault(Type type, object name = null)
        {
            return name == null ? this.serviceDelegates.GetOrDefault(type)
                : this.keyedServiceDelegates.GetOrDefault(this.GetKey(type, name));
        }

        public void AddFactoryDelegate(Type type, Func<IResolutionScope, Delegate> factory, object name = null)
        {
            if (name == null)
                this.factoryDelegates.AddOrUpdate(type, factory);
            else
                this.keyedFactoryDelegates.AddOrUpdate(this.GetKey(type, name), factory);
        }
        public void AddServiceDelegate(Type type, Func<IResolutionScope, object> factory, object name = null)
        {
            if (name == null)
                this.serviceDelegates.AddOrUpdate(type, factory);
            else
                this.keyedServiceDelegates.AddOrUpdate(this.GetKey(type, name), factory);
        }

        public void InvalidateDelegateCache()
        {
            this.serviceDelegates.Clear();
            this.keyedServiceDelegates.Clear();
            this.factoryDelegates.Clear();
        }

        private object GetKey(Type type, object name) =>
            NameGenerator.GetRegistrationName(type, name);
    }
}
