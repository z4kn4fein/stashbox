using System;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Utils;

namespace Stashbox.Resolution
{
    internal class DelegateRepository : IDelegateRepository
    {
        private readonly IStashboxContainer container;
        private readonly RandomAccessArray<string, Func<object>> serviceDelegates;
        private readonly RandomAccessArray<string, Func<IStashboxContainer, object>> containerDelegate;

        public DelegateRepository(IStashboxContainer container)
        {
            this.container = container;
            this.serviceDelegates = new RandomAccessArray<string, Func<object>>();
            this.containerDelegate = new RandomAccessArray<string, Func<IStashboxContainer, object>>();
        }

        public object ActivateFromDelegateCacheOrDefault(TypeInformation typeInfo)
        {
            var key = this.GetKey(typeInfo);
            var factory = this.serviceDelegates.Load(key);
            return factory == null ? this.containerDelegate.Load(key)?.Invoke(this.container) : factory.Invoke();
        }

        public void AddServiceDelegate(TypeInformation typeInfo, Func<object> factory)
        {
            var key = this.GetKey(typeInfo);
            this.serviceDelegates.Store(key, factory);
        }

        public void AddContainereDelegate(TypeInformation typeInfo, Func<IStashboxContainer, object> factory)
        {
            var key = this.GetKey(typeInfo);
            this.containerDelegate.Store(key, factory);
        }

        private string GetKey(TypeInformation typeInfo) => typeInfo.Type.FullName + typeInfo.DependencyName;
    }
}
