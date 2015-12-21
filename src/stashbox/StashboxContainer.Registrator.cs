using Ronin.Common;
using Stashbox.BuildUp;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Lifetime;
using Stashbox.MetaInfo;
using Stashbox.Registration;
using Stashbox.Utils;
using System;
using System.Reflection;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        public IDependencyRegistrator RegisterType<TFrom, TTo>(string name = null) where TFrom : class where TTo : class, TFrom
        {
            this.RegisterTypeInternal(typeof(TTo), typeof(TFrom), name);
            return this;
        }

        public IDependencyRegistrator RegisterType<TTo>(Type typeTo, string name = null) where TTo : class
        {
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            this.RegisterTypeInternal(typeTo, typeof(TTo), name);
            return this;
        }

        public IDependencyRegistrator RegisterType(Type typeFrom, Type typeTo, string name = null)
        {
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            this.RegisterTypeInternal(typeTo, typeFrom, name);
            return this;
        }

        public IDependencyRegistrator RegisterType<TTo>(string name = null) where TTo : class
        {
            var type = typeof(TTo);
            this.RegisterTypeInternal(type, type, name);
            return this;
        }

        public IDependencyRegistrator RegisterInstance<TFrom>(object instance, string name = null) where TFrom : class
        {
            Shield.EnsureNotNull(instance, nameof(instance));
            this.RegisterInstanceInternal(instance, name, typeof(TFrom));
            return this;
        }

        public IDependencyRegistrator RegisterInstance(object instance, Type type = null, string name = null)
        {
            Shield.EnsureNotNull(instance, nameof(instance));
            this.RegisterInstanceInternal(instance, name, type);
            return this;
        }

        public IDependencyRegistrator BuildUp<TFrom>(object instance, string name = null) where TFrom : class
        {
            Shield.EnsureNotNull(instance, nameof(instance));
            this.BuildUpInternal(instance, name, typeof(TFrom));
            return this;
        }

        public IDependencyRegistrator BuildUp(object instance, Type type = null, string name = null)
        {
            Shield.EnsureNotNull(instance, nameof(instance));
            this.BuildUpInternal(instance, name, type);
            return this;
        }

        public IRegistrationContext PrepareType<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom
        {
            return new RegistrationContext(typeof(TFrom), typeof(TTo), this.ContainerContext, this.containerExtensionManager);
        }

        public IRegistrationContext PrepareType<TFrom>(Type typeTo)
            where TFrom : class
        {
            return new RegistrationContext(typeof(TFrom), typeTo, this.ContainerContext, this.containerExtensionManager);
        }

        public IRegistrationContext PrepareType(Type typeFrom, Type typeTo)
        {
            return new RegistrationContext(typeFrom, typeTo, this.ContainerContext, this.containerExtensionManager);
        }

        public IRegistrationContext PrepareType<TTo>()
             where TTo : class
        {
            var type = typeof(TTo);
            return new RegistrationContext(type, type, this.ContainerContext, this.containerExtensionManager);
        }

        private void RegisterTypeInternal(Type typeTo, Type typeFrom, string name = null)
        {
            name = NameGenerator.GetRegistrationName(typeTo, name);

            var registrationInfo = new RegistrationInfo { TypeFrom = typeFrom, TypeTo = typeTo };
            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, typeTo);

            IObjectBuilder objectBuilder;
            if (typeTo.GetTypeInfo().IsGenericTypeDefinition)
                objectBuilder = new GenericTypeObjectBuilder(this.ContainerContext, metaInfoProvider);
            else
                objectBuilder = new DefaultObjectBuilder(this.ContainerContext, metaInfoProvider, this.containerExtensionManager);

            var registration = new ServiceRegistration(new TransientLifetime(), objectBuilder);

            this.registrationRepository.AddRegistration(typeFrom, registration, name);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, registrationInfo);
        }

        private void BuildUpInternal(object instance, string keyName, Type type = null)
        {
            type = type ?? instance.GetType();
            keyName = NameGenerator.GetRegistrationName(type, keyName);

            var registrationInfo = new RegistrationInfo { TypeFrom = type, TypeTo = type };

            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, type);
            var objectExtender = new ObjectExtender(metaInfoProvider);

            var registration = new ServiceRegistration(new TransientLifetime(),
                new BuildUpObjectBuilder(instance, this.ContainerContext, this.containerExtensionManager, objectExtender));

            this.registrationRepository.AddRegistration(type, registration, keyName);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, registrationInfo);
        }

        private void RegisterInstanceInternal(object instance, string keyName, Type type = null)
        {
            type = type ?? instance.GetType();
            keyName = NameGenerator.GetRegistrationName(type, keyName);

            var registrationInfo = new RegistrationInfo { TypeFrom = type, TypeTo = type };

            var registration = new ServiceRegistration(new TransientLifetime(),
                new InstanceObjectBuilder(instance));

            this.registrationRepository.AddRegistration(type, registration, keyName);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, registrationInfo);
        }
    }
}
