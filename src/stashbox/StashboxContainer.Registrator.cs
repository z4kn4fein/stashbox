using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Registration;
using Stashbox.Utils;
using System;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IDependencyRegistrator RegisterType<TFrom, TTo>(Action<IFluentServiceRegistrator> configurator = null)
            where TFrom : class
            where TTo : class, TFrom =>
            this.RegisterType(typeof(TFrom), typeof(TTo), configurator);

        /// <inheritdoc />
        public IDependencyRegistrator RegisterType<TFrom>(Type typeTo, Action<IFluentServiceRegistrator> configurator = null)
            where TFrom : class =>
            this.RegisterType(typeof(TFrom), typeTo, configurator);

        /// <inheritdoc />
        public IDependencyRegistrator RegisterType(Type typeFrom, Type typeTo, Action<IFluentServiceRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var context = this.ServiceRegistrator.PrepareContext(typeFrom, typeTo);
            configurator?.Invoke(context);
            return context.Register();
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterType<TTo>(Action<IFluentServiceRegistrator> configurator = null)
             where TTo : class
        {
            var type = typeof(TTo);
            return this.RegisterType(type, type, configurator);
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterType(Type typeTo, Action<IFluentServiceRegistrator> configurator = null) =>
            this.RegisterType(typeTo, typeTo, configurator);

        /// <inheritdoc />
        public IDependencyRegistrator RegisterInstanceAs<TFrom>(TFrom instance, object name = null, bool withoutDisposalTracking = false) =>
            this.RegisterInstance(typeof(TFrom), instance, name, withoutDisposalTracking);

        /// <inheritdoc />
        public IDependencyRegistrator RegisterInstance(object instance, object name = null, bool withoutDisposalTracking = false) =>
            this.RegisterInstance(instance.GetType(), instance, name, withoutDisposalTracking);

        /// <inheritdoc />
        public IDependencyRegistrator RegisterInstance(Type serviceType, object instance, object name = null, bool withoutDisposalTracking = false)
        {
            Shield.EnsureNotNull(instance, nameof(instance));

            return this.RegisterType(serviceType, instance.GetType(), context =>
            {
                context.WithInstance(instance).WithName(name);
                if (withoutDisposalTracking)
                    context.WithoutDisposalTracking();
            });
        }

        /// <inheritdoc />
        public IDependencyRegistrator WireUpAs<TFrom>(TFrom instance, object name = null, bool withoutDisposalTracking = false) =>
            this.WireUp(typeof(TFrom), instance, name, withoutDisposalTracking);

        /// <inheritdoc />
        public IDependencyRegistrator WireUp(object instance, object name = null, bool withoutDisposalTracking = false) =>
            this.WireUp(instance.GetType(), instance, name, withoutDisposalTracking);

        /// <inheritdoc />
        public IDependencyRegistrator WireUp(Type serviceType, object instance, object name = null, bool withoutDisposalTracking = false)
        {
            Shield.EnsureNotNull(instance, nameof(instance));
            Shield.EnsureNotNull(serviceType, nameof(serviceType));

            this.WireUpInternal(instance, name, serviceType, instance.GetType(), withoutDisposalTracking);
            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterSingleton<TFrom, TTo>(object name = null)
            where TFrom : class
            where TTo : class, TFrom =>
            this.RegisterType<TFrom, TTo>(context => context.WithName(name).WithSingletonLifetime());

        /// <inheritdoc />
        public IDependencyRegistrator RegisterSingleton<TTo>(object name = null)
            where TTo : class =>
            this.RegisterType<TTo>(context => context.WithName(name).WithSingletonLifetime());

        /// <inheritdoc />
        public IDependencyRegistrator RegisterSingleton(Type typeFrom, Type typeTo, object name = null) =>
            this.RegisterType(typeFrom, typeTo, context => context.WithName(name).WithSingletonLifetime());

        /// <inheritdoc />
        public IDependencyRegistrator RegisterScoped<TFrom, TTo>(object name = null)
            where TFrom : class
            where TTo : class, TFrom =>
            this.RegisterType<TFrom, TTo>(context => context.WithName(name).WithScopedLifetime());

        /// <inheritdoc />
        public IDependencyRegistrator RegisterScoped(Type typeFrom, Type typeTo, object name = null) =>
            this.RegisterType(typeFrom, typeTo, context => context.WithName(name).WithScopedLifetime());

        /// <inheritdoc />
        public IDependencyRegistrator RegisterScoped<TTo>(object name = null)
            where TTo : class =>
            this.RegisterType<TTo>(context => context.WithName(name).WithScopedLifetime());

        private void WireUpInternal(object instance, object keyName, Type typeFrom, Type typeTo, bool withoutDisposalTracking)
        {
            var data = RegistrationContextData.New();
            data.Name = keyName;
            data.ExistingInstance = instance;

            var registration = new ServiceRegistration(typeFrom, typeTo,
                this.ContainerContext, this.objectBuilderSelector.Get(ObjectBuilder.WireUp),
                data, false, !withoutDisposalTracking);

            this.registrationRepository.AddOrUpdateRegistration(registration, keyName ?? typeTo, false, false);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, typeTo, typeFrom);
        }
    }
}
