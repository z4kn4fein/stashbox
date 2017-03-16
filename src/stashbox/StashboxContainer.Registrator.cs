using Stashbox.BuildUp;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Lifetime;
using Stashbox.MetaInfo;
using Stashbox.Registration;
using Stashbox.Utils;
using System;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IDependencyRegistrator RegisterType<TFrom, TTo>(string name = null) where TFrom : class where TTo : class, TFrom
        {
            this.PrepareType<TFrom, TTo>().WithName(name).Register();
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterType<TFrom>(Type typeTo, string name = null) where TFrom : class
        {
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            this.PrepareType<TFrom>(typeTo).WithName(name).Register();
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterType(Type typeFrom, Type typeTo, string name = null)
        {
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            this.PrepareType(typeFrom, typeTo).WithName(name).Register();
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterType<TTo>(string name = null) where TTo : class
        {
            this.PrepareType<TTo>().WithName(name).Register();
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator ReMap<TFrom, TTo>(string name = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.PrepareType<TFrom, TTo>().WithName(name).ReMap();
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator ReMap<TFrom>(Type typeTo, string name = null)
            where TFrom : class
        {
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            this.PrepareType<TFrom>(typeTo).WithName(name).ReMap();
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator ReMap(Type typeFrom, Type typeTo, string name = null)
        {
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            this.PrepareType(typeFrom, typeTo).WithName(name).ReMap();
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterInstance<TFrom>(object instance, string name = null) where TFrom : class
        {
            Shield.EnsureNotNull(instance, nameof(instance));
            this.RegisterInstanceInternal(instance, name, typeof(TFrom));
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterInstance(object instance, string name = null)
        {
            Shield.EnsureNotNull(instance, nameof(instance));
            this.RegisterInstanceInternal(instance, name, instance.GetType());
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator WireUp<TFrom>(object instance, string name = null) where TFrom : class
        {
            Shield.EnsureNotNull(instance, nameof(instance));
            this.WireUpInternal(instance, name, typeof(TFrom), instance.GetType());
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator WireUp(object instance, string name = null)
        {
            Shield.EnsureNotNull(instance, nameof(instance));
            var type = instance.GetType();
            this.WireUpInternal(instance, name, type, type);
            return this;
        }

        /// <inheritdoc />
        public IRegistrationContext PrepareType<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom
        {
            return new RegistrationContext(typeof(TFrom), typeof(TTo), this.ContainerContext, this.expressionBuilder, this.containerExtensionManager);
        }

        /// <inheritdoc />
        public IRegistrationContext PrepareType<TFrom>(Type typeTo)
            where TFrom : class
        {
            return new RegistrationContext(typeof(TFrom), typeTo, this.ContainerContext, this.expressionBuilder, this.containerExtensionManager);
        }

        /// <inheritdoc />
        public IRegistrationContext PrepareType(Type typeFrom, Type typeTo)
        {
            return new RegistrationContext(typeFrom, typeTo, this.ContainerContext, this.expressionBuilder, this.containerExtensionManager);
        }

        /// <inheritdoc />
        public IRegistrationContext PrepareType<TTo>()
             where TTo : class
        {
            var type = typeof(TTo);
            return new RegistrationContext(type, type, this.ContainerContext, this.expressionBuilder, this.containerExtensionManager);
        }

        /// <inheritdoc />
        public IRegistrationContext PrepareType(Type typeTo)
        {
            return new RegistrationContext(typeTo, typeTo, this.ContainerContext, this.expressionBuilder, this.containerExtensionManager);
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterSingleton<TFrom, TTo>(string name = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.PrepareType<TFrom, TTo>().WithName(name).WithLifetime(new SingletonLifetime()).Register();
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterSingleton<TTo>(string name = null)
            where TTo : class
        {
            this.PrepareType<TTo>().WithName(name).WithLifetime(new SingletonLifetime()).Register();
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterSingleton(Type typeFrom, Type typeTo, string name = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            this.PrepareType(typeFrom, typeTo).WithName(name).WithLifetime(new SingletonLifetime()).Register();
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterScoped<TFrom, TTo>(string name = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.PrepareType<TFrom, TTo>().WithName(name).WithLifetime(new ScopedLifetime()).Register();
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterScoped(Type typeFrom, Type typeTo, string name = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            this.PrepareType(typeFrom, typeTo).WithName(name).WithLifetime(new ScopedLifetime()).Register();
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterScoped<TTo>(string name = null)
            where TTo : class
        {
            this.PrepareType<TTo>().WithName(name).WithLifetime(new ScopedLifetime()).Register();
            return this;
        }

        private void WireUpInternal(object instance, string keyName, Type typeFrom, Type typeTo)
        {
            var regName = NameGenerator.GetRegistrationName(typeFrom, typeTo, keyName);
            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, RegistrationContextData.Empty, typeTo);
            var registration = new ServiceRegistration(typeFrom, typeTo, this.ContainerContext, new TransientLifetime(),
                new WireUpObjectBuilder(instance, this.containerExtensionManager, this.ContainerContext, metaInfoProvider, this.expressionBuilder), metaInfoProvider);

            this.registrationRepository.AddOrUpdateRegistration(typeFrom, regName, false, registration);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, typeTo, typeFrom);
        }

        private void RegisterInstanceInternal(object instance, string keyName, Type type)
        {
            var instanceType = instance.GetType();
            var regName = NameGenerator.GetRegistrationName(instanceType, instanceType, keyName);
            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, RegistrationContextData.Empty, instanceType);
            var registration = new ServiceRegistration(type, instanceType, this.ContainerContext, new TransientLifetime(),
                new InstanceObjectBuilder(instance, this.ContainerContext), metaInfoProvider);

            this.registrationRepository.AddOrUpdateRegistration(type, regName, false, registration);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, instanceType, type);
        }
    }
}
