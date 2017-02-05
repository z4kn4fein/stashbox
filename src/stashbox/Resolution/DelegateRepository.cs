using System;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Utils;

namespace Stashbox.Resolution
{
    internal class DelegateRepository : IDelegateRepository
    {
        private readonly ConcurrentTree<Type, Func<object>> serviceDelegates;
        private readonly ConcurrentTree<string, Func<object>> keyedServiceDelegates;

        public DelegateRepository()
        {
            this.serviceDelegates = new ConcurrentTree<Type, Func<object>>();
            this.keyedServiceDelegates = new ConcurrentTree<string, Func<object>>();
        }

        public Func<object> GetDelegateCacheOrDefault(TypeInformation typeInfo)
        {
            return typeInfo.DependencyName == null ? this.serviceDelegates.GetOrDefault(typeInfo.Type) : this.keyedServiceDelegates.GetOrDefault(this.GetKey(typeInfo));
        }

        public void AddServiceDelegate(TypeInformation typeInfo, Func<object> factory)
        {
            if (typeInfo.DependencyName == null)
                this.serviceDelegates.AddOrUpdate(typeInfo.Type, factory);
            
            this.keyedServiceDelegates.AddOrUpdate(this.GetKey(typeInfo), factory);
        }

        public void InvalidateServiceDelegate(TypeInformation typeInfo)
        {
            if (typeInfo.DependencyName == null)
                this.serviceDelegates.AddOrUpdate(typeInfo.Type, null, (old, newVal) => newVal);
            
            this.keyedServiceDelegates.AddOrUpdate(this.GetKey(typeInfo), null, (old, newVal) => newVal);
        }

        private string GetKey(TypeInformation typeInformation) => NameGenerator.GetRegistrationName(typeInformation.Type, typeInformation.Type, typeInformation.DependencyName);
    }
}
