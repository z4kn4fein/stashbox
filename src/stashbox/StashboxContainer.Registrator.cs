using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.MetaInfo;
using Stashbox.Registration;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        public IDependencyRegistrator ReMap<TFrom, TTo>(Action<IFluentServiceRegistrator> configurator)
            where TFrom : class
            where TTo : class, TFrom =>
            this.ReMap(typeof(TFrom), typeof(TTo), configurator);

        /// <inheritdoc />
        public IDependencyRegistrator ReMap<TFrom>(Type typeTo, Action<IFluentServiceRegistrator> configurator)
            where TFrom : class =>
            this.ReMap(typeof(TFrom), typeTo, configurator);

        /// <inheritdoc />
        public IDependencyRegistrator ReMap(Type typeFrom, Type typeTo, Action<IFluentServiceRegistrator> configurator)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            Shield.EnsureNotNull(configurator, nameof(configurator));

            var context = this.ServiceRegistrator.PrepareContext(typeFrom, typeTo);
            configurator(context);
            return context.ReMap();
        }

        /// <inheritdoc />
        public IDependencyRegistrator ReMap<TTo>(Action<IFluentServiceRegistrator> configurator)
             where TTo : class
        {
            var type = typeof(TTo);
            return this.ReMap(type, type, configurator);
        }

        /// <inheritdoc />
        public IDependencyRegistrator ReMap(Type typeTo, Action<IFluentServiceRegistrator> configurator) =>
            this.ReMap(typeTo, typeTo, configurator);


        /// <inheritdoc />
        public IDependencyRegistrator ReMap<TFrom, TTo>(string name = null)
            where TFrom : class
            where TTo : class, TFrom =>
            this.ReMap<TFrom, TTo>(context => context.WithName(name));

        /// <inheritdoc />
        public IDependencyRegistrator ReMap<TFrom>(Type typeTo, string name = null)
            where TFrom : class =>
            this.ReMap<TFrom>(typeTo, context => context.WithName(name));

        /// <inheritdoc />
        public IDependencyRegistrator ReMap(Type typeFrom, Type typeTo, string name = null) =>
            this.ReMap(typeFrom, typeTo, context => context.WithName(name));

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

        /// <inheritdoc />
        public IDependencyRegistrator RegisterTypesAs<TFrom>(IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
             where TFrom : class =>
             this.RegisterTypesAs(typeof(TFrom), types, selector, configurator);

        /// <inheritdoc />
        public IDependencyRegistrator RegisterTypesAs(Type typeFrom, IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(types, nameof(types));

            if (selector != null)
                types = types.Where(selector);

            foreach (var type in types.Where(t => t.IsValidForRegistration() && t.Implements(typeFrom)))
            {
                if (configurator == null)
                    this.RegisterType(typeFrom, type);
                else
                    this.RegisterType(typeFrom, type, configurator);
            }

            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterTypesAsSelf(IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(types, nameof(types));
            Shield.EnsureNotNull(selector, nameof(selector));

            if (selector != null)
                types = types.Where(selector);

            foreach (var type in types.Where(t => t.IsValidForRegistration()))
            {
                if (configurator == null)
                    this.RegisterType(type);
                else
                    this.RegisterType(type, configurator);
            }

            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterTypes(IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(types, nameof(types));
            Shield.EnsureNotNull(selector, nameof(selector));

            if (selector != null)
                types = types.Where(selector);

            foreach (var type in types.Where(t => t.IsValidForRegistration()))
                foreach (var interfaceType in type.GetRegisterableInterfaceTypes())
                {
                    if (configurator == null)
                        this.RegisterType(interfaceType, type);
                    else
                        this.RegisterType(interfaceType, type, configurator);
                }

            return this;
        }

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
