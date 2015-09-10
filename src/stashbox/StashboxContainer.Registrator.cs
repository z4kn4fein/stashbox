using Ronin.Common;
using Stashbox.BuildUp;
using Stashbox.Entity;
using Stashbox.Entity.Events;
using Stashbox.Infrastructure;
using Stashbox.Lifetime;
using Stashbox.MetaInfo;
using Stashbox.Registration;
using System;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        public void RegisterType<TKey, TValue>(string name = null, ILifetime lifetime = null, Func<object> singleFactory = null, Func<object> oneParamsFactory = null,
            Func<object> twoParamsFactory = null, Func<object> threeParamsFactory = null) where TKey : class where TValue : class, TKey
        {
            this.RegisterTypeInternal(typeof(TValue), typeof(TKey), name, lifetime, singleFactory, oneParamsFactory, twoParamsFactory, threeParamsFactory);
        }

        public void RegisterType<TKey>(Type typeTo, string name = null, ILifetime lifetime = null, Func<object> singleFactory = null, Func<object> oneParamsFactory = null,
            Func<object> twoParamsFactory = null, Func<object> threeParamsFactory = null) where TKey : class
        {
            Shield.EnsureNotNull(typeTo);
            this.RegisterTypeInternal(typeTo, typeof(TKey), name, lifetime, singleFactory, oneParamsFactory, twoParamsFactory, threeParamsFactory);
        }

        public void RegisterType(Type typeTo, Type typeFrom = null, string name = null, ILifetime lifetime = null, Func<object> singleFactory = null, Func<object> oneParamsFactory = null,
            Func<object> twoParamsFactory = null, Func<object> threeParamsFactory = null)
        {
            Shield.EnsureNotNull(typeTo);
            this.RegisterTypeInternal(typeTo, typeFrom, name, lifetime, singleFactory, oneParamsFactory, twoParamsFactory, threeParamsFactory);
        }

        public void RegisterType<TValue>(string name = null, ILifetime lifetime = null, Func<object> singleFactory = null, Func<object> oneParamsFactory = null,
            Func<object> twoParamsFactory = null, Func<object> threeParamsFactory = null) where TValue : class
        {
            this.RegisterTypeInternal(typeof(TValue), null, name, lifetime, singleFactory, oneParamsFactory, twoParamsFactory, threeParamsFactory);
        }

        public void RegisterInstance<TKey>(object instance, string name = null) where TKey : class
        {
            Shield.EnsureNotNull(instance);
            this.RegisterInstanceInternal(instance, name, typeof(TKey));
        }

        public void RegisterInstance(object instance, Type type = null, string name = null)
        {
            Shield.EnsureNotNull(instance);
            this.RegisterInstanceInternal(instance, name, type);
        }

        public void BuildUp<TKey>(object instance, string name = null) where TKey : class
        {
            Shield.EnsureNotNull(instance);
            this.BuildUpInternal(instance, name, typeof(TKey));
        }

        public void BuildUp(object instance, Type type = null, string name = null)
        {
            Shield.EnsureNotNull(instance);
            this.BuildUpInternal(instance, name, type);
        }

        private void RegisterTypeInternal(Type typeTo, Type typeFrom, string keyName, ILifetime lifetime = null, Func<object> singleFactory = null,
            Func<object> oneParamsFactory = null, Func<object> twoParamsFactory = null, Func<object> threeParamsFactory = null)
        {
            var name = this.GetRegistrationName(keyName);
            var registrationLifetime = lifetime ?? new TransientLifetime();
            var registration = new ServiceRegistration(registrationLifetime,
                this.CreateObjectBuilder(typeTo, singleFactory, oneParamsFactory, twoParamsFactory, threeParamsFactory), this.builderContext);

            this.registrationRepository.AddRegistration(typeFrom, registration, name);

            var registrationInfo = new RegistrationInfo
            {
                TypeFrom = typeFrom,
                TypeTo = typeTo
            };

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.builderContext, registrationInfo);
            this.messagePublisher.Broadcast(new RegistrationAdded { RegistrationInfo = registrationInfo });
        }

        private void BuildUpInternal(object instance, string keyName, Type type = null)
        {
            type = type ?? instance.GetType();
            var name = this.GetRegistrationName(keyName);

            var registration = new ServiceRegistration(new TransientLifetime(),
                new BuildUpObjectBuilder(instance, this.containerExtensionManager), this.builderContext);

            this.registrationRepository.AddRegistration(type, registration, name);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.builderContext, new RegistrationInfo
            {
                TypeFrom = type,
                TypeTo = type
            });
        }

        private void RegisterInstanceInternal(object instance, string keyName, Type type = null)
        {
            type = type ?? instance.GetType();
            var name = this.GetRegistrationName(keyName);

            var registration = new ServiceRegistration(new TransientLifetime(),
                new InstanceObjectBuilder(instance), this.builderContext);

            this.registrationRepository.AddRegistration(type, registration, name);
        }

        private string GetRegistrationName(string nameKey = null)
        {
            return string.IsNullOrWhiteSpace(nameKey) ? Guid.NewGuid().ToString() : nameKey;
        }

        private IObjectBuilder CreateObjectBuilder(Type typeTo, Func<object> singleFactory = null,
            Func<object> oneParamsFactory = null, Func<object> twoParamsFactory = null, Func<object> threeParamsFactory = null)
        {
            if (singleFactory != null)
                return new FactoryObjectBuilder(singleFactory, this.containerExtensionManager);

            if (oneParamsFactory != null)
                return new FactoryObjectBuilder(oneParamsFactory, this.containerExtensionManager);

            if (twoParamsFactory != null)
                return new FactoryObjectBuilder(twoParamsFactory, this.containerExtensionManager);

            if (threeParamsFactory != null)
                return new FactoryObjectBuilder(threeParamsFactory, this.containerExtensionManager);


            return new DefaultObjectBuilder(new MetaInfoProvider(this.builderContext, this.resolverSelector, typeTo),
                this.containerExtensionManager, this.messagePublisher);
        }
    }
}
