using Ronin.Common;
using Stashbox.BuildUp;
using Stashbox.Entity;
using Stashbox.Entity.Events;
using Stashbox.Infrastructure;
using Stashbox.Lifetime;
using Stashbox.MetaInfo;
using Stashbox.Registration;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        public void RegisterType<TKey, TValue>(string name = null, ILifetime lifetime = null, Func<object> singleFactory = null, Func<object> oneParamsFactory = null,
            Func<object> twoParamsFactory = null, Func<object> threeParamsFactory = null, IEnumerable<InjectionParameter> injectionParameters = null) where TKey : class where TValue : class, TKey
        {
            this.RegisterTypeInternal(typeof(TValue), typeof(TKey), name, lifetime, singleFactory, oneParamsFactory, twoParamsFactory, threeParamsFactory, injectionParameters);
        }

        public void RegisterType<TKey>(Type typeTo, string name = null, ILifetime lifetime = null, Func<object> singleFactory = null, Func<object> oneParamsFactory = null,
            Func<object> twoParamsFactory = null, Func<object> threeParamsFactory = null, IEnumerable<InjectionParameter> injectionParameters = null) where TKey : class
        {
            Shield.EnsureNotNull(typeTo);
            this.RegisterTypeInternal(typeTo, typeof(TKey), name, lifetime, singleFactory, oneParamsFactory, twoParamsFactory, threeParamsFactory, injectionParameters);
        }

        public void RegisterType(Type typeTo, Type typeFrom = null, string name = null, ILifetime lifetime = null, Func<object> singleFactory = null, Func<object> oneParamsFactory = null,
            Func<object> twoParamsFactory = null, Func<object> threeParamsFactory = null, IEnumerable<InjectionParameter> injectionParameters = null)
        {
            Shield.EnsureNotNull(typeTo);
            this.RegisterTypeInternal(typeTo, typeFrom, name, lifetime, singleFactory, oneParamsFactory, twoParamsFactory, threeParamsFactory, injectionParameters);
        }

        public void RegisterType<TValue>(string name = null, ILifetime lifetime = null, Func<object> singleFactory = null, Func<object> oneParamsFactory = null,
            Func<object> twoParamsFactory = null, Func<object> threeParamsFactory = null, IEnumerable<InjectionParameter> injectionParameters = null) where TValue : class
        {
            this.RegisterTypeInternal(typeof(TValue), null, name, lifetime, singleFactory, oneParamsFactory, twoParamsFactory, threeParamsFactory, injectionParameters);
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
            Func<object> oneParamsFactory = null, Func<object> twoParamsFactory = null, Func<object> threeParamsFactory = null, IEnumerable<InjectionParameter> injectionParameters = null)
        {
            var name = this.GetRegistrationName(keyName);

            var registrationLifetime = lifetime ?? new TransientLifetime();

            var registrationInfo = new RegistrationInfo { TypeFrom = typeFrom, TypeTo = typeTo };

            var registration = new ServiceRegistration(registrationLifetime,
                this.CreateObjectBuilder(typeTo, singleFactory, oneParamsFactory, twoParamsFactory, threeParamsFactory, injectionParameters), this.builderContext, registrationInfo);

            this.registrationRepository.AddRegistration(typeFrom, registration, name);
            this.NotifyAboutNewRegistration(registrationInfo);
        }

        private void BuildUpInternal(object instance, string keyName, Type type = null)
        {
            type = type ?? instance.GetType();
            var name = this.GetRegistrationName(keyName);

            var registrationInfo = new RegistrationInfo { TypeFrom = type, TypeTo = type };

            var registration = new ServiceRegistration(new TransientLifetime(),
                new BuildUpObjectBuilder(instance, this.containerExtensionManager), this.builderContext, registrationInfo);

            this.registrationRepository.AddRegistration(type, registration, name);
            this.NotifyAboutNewRegistration(registrationInfo);
        }

        private void RegisterInstanceInternal(object instance, string keyName, Type type = null)
        {
            type = type ?? instance.GetType();
            var name = this.GetRegistrationName(keyName);

            var registrationInfo = new RegistrationInfo { TypeFrom = type, TypeTo = type };

            var registration = new ServiceRegistration(new TransientLifetime(),
                new InstanceObjectBuilder(instance), this.builderContext, registrationInfo);

            this.registrationRepository.AddRegistration(type, registration, name);
            this.NotifyAboutNewRegistration(registrationInfo);
        }

        private string GetRegistrationName(string nameKey = null)
        {
            return string.IsNullOrWhiteSpace(nameKey) ? Guid.NewGuid().ToString() : nameKey;
        }

        private void NotifyAboutNewRegistration(RegistrationInfo registrationInfo)
        {
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.builderContext, registrationInfo);
            this.messagePublisher.Broadcast(new RegistrationAdded { RegistrationInfo = registrationInfo });
        }

        private IObjectBuilder CreateObjectBuilder(Type typeTo, Func<object> singleFactory = null,
            Func<object> oneParamsFactory = null, Func<object> twoParamsFactory = null,
            Func<object> threeParamsFactory = null, IEnumerable<InjectionParameter> injectionParameters = null)
        {
            if (singleFactory != null)
                return new FactoryObjectBuilder(singleFactory, this.containerExtensionManager);

            if (oneParamsFactory != null)
                return new FactoryObjectBuilder(oneParamsFactory, this.containerExtensionManager);

            if (twoParamsFactory != null)
                return new FactoryObjectBuilder(twoParamsFactory, this.containerExtensionManager);

            if (threeParamsFactory != null)
                return new FactoryObjectBuilder(threeParamsFactory, this.containerExtensionManager);

            if (typeTo.GetTypeInfo().IsGenericTypeDefinition)
                return new GenericTypeObjectBuilder(new MetaInfoProvider(this.builderContext, this.resolverSelector, typeTo));

            return new DefaultObjectBuilder(new MetaInfoProvider(this.builderContext, this.resolverSelector, typeTo),
                this.containerExtensionManager, this.messagePublisher, injectionParameters);
        }
    }
}
