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
        /// <summary>
        /// Registers a type into the container.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        public IDependencyRegistrator RegisterType<TFrom, TTo>(string name = null) where TFrom : class where TTo : class, TFrom
        {
            this.RegisterTypeInternal(typeof(TTo), typeof(TFrom), name);
            return this;
        }

        /// <summary>
        /// Registers a type into the container.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        public IDependencyRegistrator RegisterType<TFrom>(Type typeTo, string name = null) where TFrom : class
        {
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            this.RegisterTypeInternal(typeTo, typeof(TFrom), name);
            return this;
        }

        /// <summary>
        /// Registers a type into the container.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        public IDependencyRegistrator RegisterType(Type typeFrom, Type typeTo, string name = null)
        {
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            this.RegisterTypeInternal(typeTo, typeFrom, name);
            return this;
        }

        /// <summary>
        /// Registers a type into the container.
        /// </summary>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        public IDependencyRegistrator RegisterType<TTo>(string name = null) where TTo : class
        {
            var type = typeof(TTo);
            this.RegisterTypeInternal(type, type, name);
            return this;
        }

        /// <summary>
        /// Replaces an existing registration with a new one.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        public IDependencyRegistrator ReMap<TFrom, TTo>(string name = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ReMapInternal(typeof(TTo), typeof(TFrom), name);
            return this;
        }

        /// <summary>
        /// Replaces an existing registration with a new one.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        public IDependencyRegistrator ReMap<TFrom>(Type typeTo, string name = null)
            where TFrom : class
        {
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            this.ReMapInternal(typeTo, typeof(TFrom), name);
            return this;
        }

        /// <summary>
        /// Replaces an existing registration with a new one.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        public IDependencyRegistrator ReMap(Type typeFrom, Type typeTo, string name = null)
        {
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            this.ReMapInternal(typeTo, typeFrom, name);
            return this;
        }

        /// <summary>
        /// Registers an already constructed instance into the container.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="instance">The constructed object.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        public IDependencyRegistrator RegisterInstance<TFrom>(object instance, string name = null) where TFrom : class
        {
            Shield.EnsureNotNull(instance, nameof(instance));
            this.RegisterInstanceInternal(instance, name, typeof(TFrom));
            return this;
        }

        /// <summary>
        /// Registers an already constructed instance into the container.
        /// </summary>
        /// <param name="instance">The constructed object.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        public IDependencyRegistrator RegisterInstance(object instance, string name = null)
        {
            Shield.EnsureNotNull(instance, nameof(instance));
            this.RegisterInstanceInternal(instance, name, instance.GetType());
            return this;
        }

        /// <summary>
        /// Registers an already constructed instance, but the container will perform injections and extensions on it.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="instance">The constructed object.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        public IDependencyRegistrator BuildUp<TFrom>(object instance, string name = null) where TFrom : class
        {
            Shield.EnsureNotNull(instance, nameof(instance));
            this.BuildUpInternal(instance, name, typeof(TFrom), instance.GetType());
            return this;
        }

        /// <summary>
        /// Registers an already constructed instance, but the container will perform injections and extensions on it.
        /// </summary>
        /// <param name="instance">The constructed object.</param>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IDependencyRegistrator"/> which on this method was called.</returns>
        public IDependencyRegistrator BuildUp(object instance, string name = null)
        {
            Shield.EnsureNotNull(instance, nameof(instance));
            var type = instance.GetType();
            this.BuildUpInternal(instance, name, type, type);
            return this;
        }

        /// <summary>
        /// Prepares a type for registration.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <returns>The created <see cref="IRegistrationContext"/> which allows further configurations.</returns>
        public IRegistrationContext PrepareType<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom
        {
            return new RegistrationContext(typeof(TFrom), typeof(TTo), this.ContainerContext, this.containerExtensionManager);
        }

        /// <summary>
        /// Prepares a type for registration.
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested.</typeparam>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <returns>The created <see cref="IRegistrationContext"/> which allows further configurations.</returns>
        public IRegistrationContext PrepareType<TFrom>(Type typeTo)
            where TFrom : class
        {
            return new RegistrationContext(typeof(TFrom), typeTo, this.ContainerContext, this.containerExtensionManager);
        }

        /// <summary>
        /// Prepares a type for registration.
        /// </summary>
        /// <param name="typeFrom">Type that will be requested.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <returns>The created <see cref="IRegistrationContext"/> which allows further configurations.</returns>
        public IRegistrationContext PrepareType(Type typeFrom, Type typeTo)
        {
            return new RegistrationContext(typeFrom, typeTo, this.ContainerContext, this.containerExtensionManager);
        }

        /// <summary>
        /// Prepares a type for registration.
        /// </summary>
        /// <typeparam name="TTo">Type that will be returned.</typeparam>
        /// <returns>The created <see cref="IRegistrationContext"/> which allows further configurations.</returns>
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
            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, this.ContainerContext.MetaInfoRepository.GetOrAdd(typeTo, () => new MetaInfoCache(typeTo)));

            if (typeTo.GetTypeInfo().IsGenericTypeDefinition)
            {
                var objectBuilder = new GenericTypeObjectBuilder(this.ContainerContext, metaInfoProvider);
                var registration = new ServiceRegistration(new TransientLifetime(), objectBuilder);
                this.registrationRepository.AddGenericDefinition(typeFrom, registration, name);
            }
            else
            {
                var objectBuilder = new DefaultObjectBuilder(this.ContainerContext, metaInfoProvider, this.containerExtensionManager, name);
                var registration = new ServiceRegistration(new TransientLifetime(), objectBuilder);
                this.registrationRepository.AddRegistration(typeFrom, registration, name);
            }

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, registrationInfo);
        }

        private void ReMapInternal(Type typeTo, Type typeFrom, string name = null)
        {
            name = NameGenerator.GetRegistrationName(typeTo, name);

            var registrationInfo = new RegistrationInfo { TypeFrom = typeFrom, TypeTo = typeTo };
            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, this.ContainerContext.MetaInfoRepository.GetOrAdd(typeTo, () => new MetaInfoCache(typeTo)));

            if (typeTo.GetTypeInfo().IsGenericTypeDefinition)
            {
                var objectBuilder = new GenericTypeObjectBuilder(this.ContainerContext, metaInfoProvider);
                var registration = new ServiceRegistration(new TransientLifetime(), objectBuilder);
                this.registrationRepository.AddOrUpdateGenericDefinition(typeFrom, registration, name);
            }
            else
            {
                var objectBuilder = new DefaultObjectBuilder(this.ContainerContext, metaInfoProvider, this.containerExtensionManager, name);
                var registration = new ServiceRegistration(new TransientLifetime(), objectBuilder);
                this.registrationRepository.AddOrUpdateRegistration(typeFrom, registration, name);
            }

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, registrationInfo);

            foreach (var serviceRegistration in this.registrationRepository.GetAllRegistrations())
                serviceRegistration.ServiceUpdated(registrationInfo);

        }

        private void BuildUpInternal(object instance, string keyName, Type typeFrom, Type typeTo)
        {
            keyName = NameGenerator.GetRegistrationName(typeTo, keyName);

            var registrationInfo = new RegistrationInfo { TypeFrom = typeFrom, TypeTo = typeTo };

            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, this.ContainerContext.MetaInfoRepository.GetOrAdd(typeTo, () => new MetaInfoCache(typeTo)));
            var objectExtender = new ObjectExtender(metaInfoProvider);

            var registration = new ServiceRegistration(new TransientLifetime(),
                new BuildUpObjectBuilder(instance, this.ContainerContext, this.containerExtensionManager, objectExtender));

            this.registrationRepository.AddRegistration(typeFrom, registration, keyName);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, registrationInfo);
        }

        private void RegisterInstanceInternal(object instance, string keyName, Type type)
        {
            keyName = NameGenerator.GetRegistrationName(type, keyName);

            var registrationInfo = new RegistrationInfo { TypeFrom = type, TypeTo = type };

            var registration = new ServiceRegistration(new TransientLifetime(),
                new InstanceObjectBuilder(instance));

            this.registrationRepository.AddRegistration(type, registration, keyName);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, registrationInfo);
        }
    }
}
