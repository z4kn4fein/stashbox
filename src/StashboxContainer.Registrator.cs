using Stashbox.Registration.Fluent;
using Stashbox.Utils;
using System;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        [Obsolete("RegisterType has been renamed to Register.")]
        public IStashboxContainer RegisterType<TFrom, TTo>(Action<IFluentServiceConfigurator<TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom =>
            this.Register<TFrom, TTo>(configurator);

        /// <inheritdoc />
        [Obsolete("RegisterType has been renamed to Register.")]
        public IStashboxContainer RegisterType<TFrom>(Type typeTo, Action<IFluentServiceConfigurator<TFrom>> configurator = null)
            where TFrom : class =>
            this.Register(typeTo, configurator);

        /// <inheritdoc />
        [Obsolete("RegisterType has been renamed to Register.")]
        public IStashboxContainer RegisterType(Type typeFrom, Type typeTo, Action<IFluentServiceConfigurator> configurator = null) =>
            this.Register(typeFrom, typeTo, configurator);

        /// <inheritdoc />
        [Obsolete("RegisterType has been renamed to Register.")]
        public IStashboxContainer RegisterType<TTo>(Action<IFluentServiceConfigurator<TTo>> configurator = null)
            where TTo : class =>
            this.Register(configurator);

        /// <inheritdoc />
        [Obsolete("RegisterType has been renamed to Register.")]
        public IStashboxContainer RegisterType(Type typeTo, Action<IFluentServiceConfigurator> configurator = null) =>
            this.Register(typeTo, typeTo, configurator);

        /// <inheritdoc />
        public IStashboxContainer Register<TFrom, TTo>(Action<IFluentServiceConfigurator<TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            var context = new RegistrationConfigurator<TTo>(typeof(TFrom), typeof(TTo));
            configurator?.Invoke(context);
            return this.Register(context);
        }

        /// <inheritdoc />
        public IStashboxContainer Register<TFrom>(Type typeTo, Action<IFluentServiceConfigurator<TFrom>> configurator = null)
            where TFrom : class
        {
            var context = new RegistrationConfigurator<TFrom>(typeof(TFrom), typeTo);
            configurator?.Invoke(context);
            return this.Register(context);
        }

        /// <inheritdoc />
        public IStashboxContainer Register(Type typeFrom, Type typeTo, Action<IFluentServiceConfigurator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var context = new RegistrationConfigurator(typeFrom, typeTo);
            configurator?.Invoke(context);
            return this.Register(context);
        }

        /// <inheritdoc />
        public IStashboxContainer Register<TTo>(Action<IFluentServiceConfigurator<TTo>> configurator = null)
            where TTo : class
        {
            var type = typeof(TTo);
            var context = new RegistrationConfigurator<TTo>(type, type);
            configurator?.Invoke(context);
            return this.Register(context);
        }

        /// <inheritdoc />
        public IStashboxContainer Register(Type typeTo, Action<IFluentServiceConfigurator> configurator = null) =>
            this.Register(typeTo, typeTo, configurator);

        /// <inheritdoc />
        public IStashboxContainer RegisterInstanceAs<TFrom>(TFrom instance, object name = null, bool withoutDisposalTracking = false,
            Action<TFrom> finalizerDelegate = null) where TFrom : class
        {
            Shield.EnsureNotNull(instance, nameof(instance));

            return this.Register<TFrom>(instance.GetType(), context =>
            {
                context.WithFinalizer(finalizerDelegate).WithInstance(instance).WithName(name);
                if (withoutDisposalTracking)
                    context.WithoutDisposalTracking();
            });
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterInstance(Type serviceType, object instance, object name = null, bool withoutDisposalTracking = false)
        {
            Shield.EnsureNotNull(instance, nameof(instance));

            return this.Register(serviceType, instance.GetType(), context =>
            {
                context.WithInstance(instance).WithName(name);
                if (withoutDisposalTracking)
                    context.WithoutDisposalTracking();
            });
        }

        /// <inheritdoc />
        public IStashboxContainer WireUpAs<TFrom>(TFrom instance, object name = null, bool withoutDisposalTracking = false,
            Action<TFrom> finalizerDelegate = null) where TFrom : class
        {
            Shield.EnsureNotNull(instance, nameof(instance));

            return this.Register<TFrom>(instance.GetType(), context =>
            {
                context.WithFinalizer(finalizerDelegate).WithInstance(instance, true).WithName(name);
                if (withoutDisposalTracking)
                    context.WithoutDisposalTracking();
            });
        }

        /// <inheritdoc />
        public IStashboxContainer WireUp(Type serviceType, object instance, object name = null, bool withoutDisposalTracking = false)
        {
            Shield.EnsureNotNull(instance, nameof(instance));
            Shield.EnsureNotNull(serviceType, nameof(serviceType));

            return this.Register(serviceType, instance.GetType(), context =>
            {
                context.WithInstance(instance, true).WithName(name);
                if (withoutDisposalTracking)
                    context.WithoutDisposalTracking();
            });
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator(Type typeFrom, Type typeTo, Action<IFluentDecoratorConfigurator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var context = new DecoratorConfigurator(new RegistrationConfigurator(typeFrom, typeTo));
            configurator?.Invoke(context);
            return this.serviceRegistrator.Register(this.registrationBuilder.BuildServiceRegistration(context.RegistrationContext, true),
                context.RegistrationContext);
        }

        private IStashboxContainer Register(IRegistrationContext registrationContext) =>
            this.serviceRegistrator.Register(this.registrationBuilder.BuildServiceRegistration(registrationContext, false),
                registrationContext);
    }
}
