using Ronin.Common;
using Stashbox.BuildUp;
using Stashbox.Entity;
using Stashbox.Entity.Events;
using Stashbox.Infrastructure;
using Stashbox.Lifetime;
using Stashbox.MetaInfo;
using Stashbox.Registration;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
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
            Shield.EnsureNotNull(typeTo);
            this.RegisterTypeInternal(typeTo, typeof(TTo), name);
            return this;
        }

        public IDependencyRegistrator RegisterType(Type typeFrom, Type typeTo, string name = null)
        {
            Shield.EnsureNotNull(typeTo);
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
            Shield.EnsureNotNull(instance);
            this.RegisterInstanceInternal(instance, name, typeof(TFrom));
            return this;
        }

        public IDependencyRegistrator RegisterInstance(object instance, Type type = null, string name = null)
        {
            Shield.EnsureNotNull(instance);
            this.RegisterInstanceInternal(instance, name, type);
            return this;
        }

        public IDependencyRegistrator BuildUp<TFrom>(object instance, string name = null) where TFrom : class
        {
            Shield.EnsureNotNull(instance);
            this.BuildUpInternal(instance, name, typeof(TFrom));
            return this;
        }

        public IDependencyRegistrator BuildUp(object instance, Type type = null, string name = null)
        {
            Shield.EnsureNotNull(instance);
            this.BuildUpInternal(instance, name, type);
            return this;
        }

        public IRegistrationContext PrepareType<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom
        {
            return new RegistrationContext(typeof(TFrom), typeof(TTo), this.builderContext, this.containerExtensionManager);
        }

        public IRegistrationContext PrepareType<TFrom>(Type typeTo)
            where TFrom : class
        {
            return new RegistrationContext(typeof(TFrom), typeTo, this.builderContext, this.containerExtensionManager);
        }

        public IRegistrationContext PrepareType(Type typeFrom, Type typeTo)
        {
            return new RegistrationContext(typeFrom, typeTo, this.builderContext, this.containerExtensionManager);
        }

        public IRegistrationContext PrepareType<TTo>()
             where TTo : class
        {
            var type = typeof(TTo);
            return new RegistrationContext(type, type, this.builderContext, this.containerExtensionManager);
        }

        private void RegisterTypeInternal(Type typeTo, Type typeFrom, string name = null)
        {
            name = NameGenerator.GetRegistrationName(name);

            var registrationInfo = new RegistrationInfo { TypeFrom = typeFrom, TypeTo = typeTo };

            IObjectBuilder objectBuilder;
            if (typeTo.GetTypeInfo().IsGenericTypeDefinition)
                objectBuilder = new GenericTypeObjectBuilder(new MetaInfoProvider(this.builderContext, typeTo));
            else
                objectBuilder = new DefaultObjectBuilder(new MetaInfoProvider(this.builderContext, typeTo), this.containerExtensionManager, this.messagePublisher);

            var registration = new ServiceRegistration(new TransientLifetime(), objectBuilder, this.builderContext, registrationInfo);

            this.registrationRepository.AddRegistration(typeFrom, registration, name);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.builderContext, registrationInfo);
            this.messagePublisher.Broadcast(new RegistrationAdded { RegistrationInfo = registrationInfo });
        }

        private void BuildUpInternal(object instance, string keyName, Type type = null)
        {
            type = type ?? instance.GetType();
            var name = NameGenerator.GetRegistrationName(keyName);

            var registrationInfo = new RegistrationInfo { TypeFrom = type, TypeTo = type };

            var registration = new ServiceRegistration(new TransientLifetime(),
                new BuildUpObjectBuilder(instance, this.containerExtensionManager), this.builderContext, registrationInfo);

            this.registrationRepository.AddRegistration(type, registration, name);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.builderContext, registrationInfo);
            this.messagePublisher.Broadcast(new RegistrationAdded { RegistrationInfo = registrationInfo });
        }

        private void RegisterInstanceInternal(object instance, string keyName, Type type = null)
        {
            type = type ?? instance.GetType();
            var name = NameGenerator.GetRegistrationName(keyName);

            var registrationInfo = new RegistrationInfo { TypeFrom = type, TypeTo = type };

            var registration = new ServiceRegistration(new TransientLifetime(),
                new InstanceObjectBuilder(instance), this.builderContext, registrationInfo);

            this.registrationRepository.AddRegistration(type, registration, name);
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
                return new GenericTypeObjectBuilder(new MetaInfoProvider(this.builderContext, typeTo));

            return new DefaultObjectBuilder(new MetaInfoProvider(this.builderContext, typeTo),
                this.containerExtensionManager, this.messagePublisher, injectionParameters);
        }
    }
}
