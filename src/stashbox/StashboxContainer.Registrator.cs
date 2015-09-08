using Ronin.Common;
using Stashbox.BuildUp;
using Stashbox.Entity;
using Stashbox.Entity.Events;
using Stashbox.Lifetime;
using Stashbox.MetaInfo;
using Stashbox.Registration;
using System;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        public ServiceRegistration CreateRegistration<TKey, TValue>(string name = null) where TKey : class where TValue : class, TKey
        {
            throw new NotImplementedException();
        }

        public ServiceRegistration CreateRegistration<TKey>(Type valueType, string name = null) where TKey : class
        {
            throw new NotImplementedException();
        }

        public ServiceRegistration CreateRegistration(Type valueType, Type keType = null, string name = null)
        {
            throw new NotImplementedException();
        }

        public ServiceRegistration CreateRegistration<TValue>(string name = null) where TValue : class
        {
            throw new NotImplementedException();
        }

        public void RegisterType<TKey, TValue>(string name = null) where TKey : class where TValue : class, TKey
        {
            this.RegisterTypeInternal(typeof(TValue), typeof(TKey), name);
        }

        public void RegisterType<TKey>(Type typeTo, string name = null) where TKey : class
        {
            Shield.EnsureNotNull(typeTo);
            this.RegisterTypeInternal(typeTo, typeof(TKey), name);
        }

        public void RegisterType(Type typeTo, Type typeFrom = null, string name = null)
        {
            Shield.EnsureNotNull(typeTo);
            this.RegisterTypeInternal(typeTo, typeFrom, name);
        }

        public void RegisterType<TValue>(string name = null) where TValue : class
        {
            this.RegisterTypeInternal(typeof(TValue), null, name);
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

        private void RegisterTypeInternal(Type typeTo, Type typeFrom, string keyName)
        {
            var name = this.GetRegistrationName(keyName);

            var builderContext = this.builderContext;

            var registration = new ServiceRegistration(new TransientLifetime(),
                new DefaultObjectBuilder(new MetaInfoProvider(builderContext, this.resolverSelector, typeTo),
                    this.buildExtensionManager, this.messagePublisher), builderContext);

            this.registrationRepository.AddRegistration(typeFrom, registration, name);

            var registrationInfo = new RegistrationInfo
            {
                TypeFrom = typeFrom,
                TypeTo = typeTo
            };

            this.buildExtensionManager.ExecuteOnRegistrationExtensions(this.builderContext, registrationInfo);
            this.messagePublisher.Broadcast(new RegistrationAdded { RegistrationInfo = registrationInfo });
        }

        private void BuildUpInternal(object instance, string keyName, Type type = null)
        {
            type = type ?? instance.GetType();
            var name = this.GetRegistrationName(keyName);

            var registration = new ServiceRegistration(new TransientLifetime(),
                new BuildUpObjectBuilder(instance, this.buildExtensionManager), this.builderContext);

            this.registrationRepository.AddRegistration(type, registration, name);
            this.buildExtensionManager.ExecuteOnRegistrationExtensions(this.builderContext, new RegistrationInfo
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
    }
}
