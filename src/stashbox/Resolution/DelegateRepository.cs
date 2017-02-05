using System;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Utils;

namespace Stashbox.Resolution
{
    internal class DelegateRepository : IDelegateRepository
    {
        private readonly ConcurrentTree<string, Func<object>> serviceDelegates;

        public DelegateRepository()
        {
            this.serviceDelegates = new ConcurrentTree<string, Func<object>>();
        }

        public object ActivateFromDelegateCacheOrDefault(TypeInformation typeInfo)
        {
            var key = GetKey(typeInfo);
            return this.serviceDelegates.GetOrDefault(key);
        }

        public void AddServiceDelegate(TypeInformation typeInfo, Func<object> factory)
        {
            var key = GetKey(typeInfo);
            this.serviceDelegates.AddOrUpdate(key, factory);
        }

        public void InvalidateServiceDelegate(TypeInformation typeInfo)
        {
            var key = GetKey(typeInfo);
            this.serviceDelegates.AddOrUpdate(key, null, (old, newVal) => newVal);
        }

        private string GetKey(TypeInformation typeInfo) => typeInfo.Type.FullName + typeInfo.DependencyName;
    }
}
