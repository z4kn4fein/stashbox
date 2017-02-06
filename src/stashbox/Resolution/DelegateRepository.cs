using System;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Utils;

namespace Stashbox.Resolution
{
    internal class DelegateRepository : IDelegateRepository
    {
        private readonly HashMap<Type, Func<object>> serviceDelegates;
        private readonly HashMap<string, Func<object>> keyedServiceDelegates;

        public DelegateRepository()
        {
            this.serviceDelegates = new HashMap<Type, Func<object>>();
            this.keyedServiceDelegates = new HashMap<string, Func<object>>();
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
