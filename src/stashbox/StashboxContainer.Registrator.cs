using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.MetaInfo;
using Stashbox.Registration;
using Stashbox.Utils;
using System;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IDependencyRegistrator RegisterType<TFrom, TTo>(string name = null) where TFrom : class where TTo : class, TFrom =>
            this.RegisterType<TFrom, TTo>(context => context.WithName(name));

        /// <inheritdoc />
        public IDependencyRegistrator RegisterType<TFrom>(Type typeTo, string name = null) where TFrom : class =>
            this.RegisterType<TFrom>(typeTo, context => context.WithName(name));

        /// <inheritdoc />
        public IDependencyRegistrator RegisterType(Type typeFrom, Type typeTo, string name = null) =>
            this.RegisterType(typeFrom, typeTo, context => context.WithName(name));

        /// <inheritdoc />
        public IDependencyRegistrator RegisterType<TTo>(string name = null) where TTo : class =>
            this.RegisterType<TTo>(context => context.WithName(name));

        /// <inheritdoc />
        public IDependencyRegistrator RegisterType(Type typeTo, string name = null) =>
            this.RegisterType(typeTo, context => context.WithName(name));

        /// <inheritdoc />
        public IDependencyRegistrator RegisterType<TFrom, TTo>(Action<IFluentServiceRegistrator> configurator)
            where TFrom : class
            where TTo : class, TFrom =>
            this.RegisterType(typeof(TFrom), typeof(TTo), configurator);

        /// <inheritdoc />
        public IDependencyRegistrator RegisterType<TFrom>(Type typeTo, Action<IFluentServiceRegistrator> configurator)
            where TFrom : class =>
            this.RegisterType(typeof(TFrom), typeTo, configurator);

        /// <inheritdoc />
        public IDependencyRegistrator RegisterType(Type typeFrom, Type typeTo, Action<IFluentServiceRegistrator> configurator)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            Shield.EnsureNotNull(configurator, nameof(configurator));

            var context = this.ServiceRegistrator.PrepareContext(typeFrom, typeTo);
            configurator(context);
            return context.Register();
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterType<TTo>(Action<IFluentServiceRegistrator> configurator)
             where TTo : class
        {
            var type = typeof(TTo);
            return this.RegisterType(type, type, configurator);
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterType(Type typeTo, Action<IFluentServiceRegistrator> configurator) =>
            this.RegisterType(typeTo, typeTo, configurator);
        
        /// <inheritdoc />
        public IDependencyRegistrator RegisterInstance<TFrom>(object instance, string name = null, bool withoutDisposalTracking = false) where TFrom : class
        {
            Shield.EnsureNotNull(instance, nameof(instance));

            this.RegisterInstanceInternal(instance, name, typeof(TFrom), withoutDisposalTracking);
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterInstance(object instance, string name = null, bool withoutDisposalTracking = false)
        {
            Shield.EnsureNotNull(instance, nameof(instance));

            this.RegisterInstanceInternal(instance, name, instance.GetType(), withoutDisposalTracking);
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator WireUp<TFrom>(object instance, string name = null, bool withoutDisposalTracking = false) where TFrom : class
        {
            Shield.EnsureNotNull(instance, nameof(instance));

            this.WireUpInternal(instance, name, typeof(TFrom), instance.GetType(), withoutDisposalTracking);
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator WireUp(object instance, string name = null, bool withoutDisposalTracking = false)
        {
            Shield.EnsureNotNull(instance, nameof(instance));

            var type = instance.GetType();
            this.WireUpInternal(instance, name, type, type, withoutDisposalTracking);
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterSingleton<TFrom, TTo>(string name = null)
            where TFrom : class
            where TTo : class, TFrom =>
            this.RegisterType<TFrom, TTo>(context => context.WithName(name).WithSingletonLifetime());

        /// <inheritdoc />
        public IDependencyRegistrator RegisterSingleton<TTo>(string name = null)
            where TTo : class =>
            this.RegisterType<TTo>(context => context.WithName(name).WithSingletonLifetime());

        /// <inheritdoc />
        public IDependencyRegistrator RegisterSingleton(Type typeFrom, Type typeTo, string name = null) =>
            this.RegisterType(typeFrom, typeTo, context => context.WithName(name).WithSingletonLifetime());

        /// <inheritdoc />
        public IDependencyRegistrator RegisterScoped<TFrom, TTo>(string name = null)
            where TFrom : class
            where TTo : class, TFrom =>
            this.RegisterType<TFrom, TTo>(context => context.WithName(name).WithScopedLifetime());

        /// <inheritdoc />
        public IDependencyRegistrator RegisterScoped(Type typeFrom, Type typeTo, string name = null) =>
            this.RegisterType(typeFrom, typeTo, context => context.WithName(name).WithScopedLifetime());

        /// <inheritdoc />
        public IDependencyRegistrator RegisterScoped<TTo>(string name = null)
            where TTo : class =>
            this.RegisterType<TTo>(context => context.WithName(name).WithScopedLifetime());

        private void WireUpInternal(object instance, string keyName, Type typeFrom, Type typeTo, bool withoutDisposalTracking)
        {
            var regName = NameGenerator.GetRegistrationName(typeFrom, typeTo, keyName);

            var data = RegistrationContextData.New();
            data.ExistingInstance = instance;

            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, data, typeTo);

            var registration = new ServiceRegistration(typeFrom, typeTo,
                this.ContainerContext.ReserveRegistrationNumber(),
                this.objectBuilderSelector.Get(ObjectBuilder.WireUp), metaInfoProvider, data,
                false, !withoutDisposalTracking);

            this.registrationRepository.AddOrUpdateRegistration(typeFrom, regName, false, registration);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, typeTo, typeFrom);
        }

        private void RegisterInstanceInternal(object instance, string keyName, Type type, bool withoutDisposalTracking)
        {
            var instanceType = instance.GetType();

            var data = RegistrationContextData.New();
            data.ExistingInstance = instance;

            var regName = NameGenerator.GetRegistrationName(instanceType, instanceType, keyName);
            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, data, instanceType);

            var registration = new ServiceRegistration(type, instanceType, this.ContainerContext.ReserveRegistrationNumber(),
                this.objectBuilderSelector.Get(ObjectBuilder.Instance), metaInfoProvider, data, false, !withoutDisposalTracking);

            this.registrationRepository.AddOrUpdateRegistration(type, regName, false, registration);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, instanceType, type);
        }
    }
}
