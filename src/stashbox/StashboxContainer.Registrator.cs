using Stashbox.BuildUp;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Lifetime;
using Stashbox.MetaInfo;
using Stashbox.Registration;
using Stashbox.Utils;
using System;
using Stashbox.Infrastructure.Registration;

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
            return new RegistrationContext(typeof(TFrom), typeof(TTo), this.ContainerContext, this.containerExtensionManager);
        }

        /// <inheritdoc />
        public IRegistrationContext PrepareType<TFrom>(Type typeTo)
            where TFrom : class
        {
            return new RegistrationContext(typeof(TFrom), typeTo, this.ContainerContext, this.containerExtensionManager);
        }

        /// <inheritdoc />
        public IRegistrationContext PrepareType(Type typeFrom, Type typeTo)
        {
            return new RegistrationContext(typeFrom, typeTo, this.ContainerContext, this.containerExtensionManager);
        }

        /// <inheritdoc />
        public IRegistrationContext PrepareType<TTo>()
             where TTo : class
        {
            var type = typeof(TTo);
            return new RegistrationContext(type, type, this.ContainerContext, this.containerExtensionManager);
        }

        /// <inheritdoc />
        public IRegistrationContext PrepareType(Type typeTo)
        {
            return new RegistrationContext(typeTo, typeTo, this.ContainerContext, this.containerExtensionManager);
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

            var registrationInfo = new RegistrationInfo { TypeFrom = typeFrom, TypeTo = typeTo };

            var cache = new MetaInfoCache(this.ContainerContext.ContainerConfigurator, typeTo);
            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, cache);

            var registration = new ServiceRegistration(regName, typeFrom, this.ContainerContext, new TransientLifetime(),
                new WireUpObjectBuilder(instance, this.containerExtensionManager, this.ContainerContext,  metaInfoProvider), metaInfoProvider);

            this.registrationRepository.AddOrUpdateRegistration(typeFrom, regName, false, registration);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, registrationInfo);
        }

        private void RegisterInstanceInternal(object instance, string keyName, Type type)
        {
            var insanceType = instance.GetType();
            var regName = NameGenerator.GetRegistrationName(insanceType, insanceType, keyName);

            var registrationInfo = new RegistrationInfo { TypeFrom = type, TypeTo = type };
            var cache = new MetaInfoCache(this.ContainerContext.ContainerConfigurator, insanceType);
            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, cache);
            var registration = new ServiceRegistration(regName, type, this.ContainerContext, new TransientLifetime(),
                new InstanceObjectBuilder(instance), metaInfoProvider);

            this.registrationRepository.AddOrUpdateRegistration(type, regName, false, registration);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, registrationInfo);
        }
    }
}
