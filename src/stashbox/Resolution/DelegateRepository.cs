using System;
using Stashbox.Infrastructure;
using Stashbox.Utils;

namespace Stashbox.Resolution
{
    internal class DelegateRepository : IDelegateRepository
    {
        private readonly HashMap<object, Func<IResolutionScope, object>> serviceDelegates;
        private readonly HashMap<object, Func<IResolutionScope, Delegate>> factoryDelegates;

        public DelegateRepository()
        {
            this.serviceDelegates = new HashMap<object, Func<IResolutionScope, object>>();
            this.factoryDelegates = new HashMap<object, Func<IResolutionScope, Delegate>>();
        }

        public Func<IResolutionScope, object> GetDelegateCacheOrDefault(Type type) =>
            this.serviceDelegates.GetOrDefault(type);

        public Func<IResolutionScope, object> GetDelegateCacheOrDefault(object name) =>
            this.serviceDelegates.GetOrDefault(name);

        public Func<IResolutionScope, Delegate> GetFactoryDelegateCacheOrDefault(Type type, object name = null) =>
            this.factoryDelegates.GetOrDefault(name ?? type);

        public void AddFactoryDelegate(Type type, Func<IResolutionScope, Delegate> factory, object name = null) =>
            this.factoryDelegates.AddOrUpdate(name ?? type, factory);

        public void AddServiceDelegate(Type type, Func<IResolutionScope, object> factory, object name = null) =>
            this.serviceDelegates.AddOrUpdate(name ?? type, factory);

        public void InvalidateDelegateCache()
        {
            this.serviceDelegates.Clear();
            this.factoryDelegates.Clear();
        }
    }
}
