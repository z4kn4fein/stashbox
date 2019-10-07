using Stashbox.Registration.Fluent;
using Stashbox.Utils;
using System;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IStashboxContainer Register<TFrom, TTo>(Action<RegistrationConfigurator<TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            var registrationConfigurator = new RegistrationConfigurator<TTo>(typeof(TFrom), typeof(TTo));
            configurator?.Invoke(registrationConfigurator);
            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer Register<TFrom>(Type typeTo, Action<RegistrationConfigurator<TFrom>> configurator = null)
            where TFrom : class
        {
            var registrationConfigurator = new RegistrationConfigurator<TFrom>(typeof(TFrom), typeTo);
            configurator?.Invoke(registrationConfigurator);
            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer Register(Type typeFrom, Type typeTo, Action<RegistrationConfigurator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var registrationConfigurator = new RegistrationConfigurator(typeFrom, typeTo);
            configurator?.Invoke(registrationConfigurator);
            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer Register<TTo>(Action<RegistrationConfigurator<TTo>> configurator = null)
            where TTo : class
        {
            var type = typeof(TTo);
            var registrationConfigurator = new RegistrationConfigurator<TTo>(type, type);
            configurator?.Invoke(registrationConfigurator);
            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer Register(Type typeTo, Action<RegistrationConfigurator> configurator = null) =>
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
        public IStashboxContainer RegisterDecorator(Type typeFrom, Type typeTo, Action<DecoratorConfigurator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var decoratorConfigurator = new DecoratorConfigurator(typeFrom, typeTo);
            configurator?.Invoke(decoratorConfigurator);
            this.serviceRegistrator.Register(
                this.ContainerContext,
                this.registrationBuilder.BuildServiceRegistration(decoratorConfigurator, true),
                typeFrom,
                decoratorConfigurator.Context);

            return this;
        }

        private IStashboxContainer RegisterInternal(RegistrationConfiguration registrationConfiguration)
        {
            this.serviceRegistrator.Register(
                this.ContainerContext,
                this.registrationBuilder.BuildServiceRegistration(registrationConfiguration, false),
                registrationConfiguration.ServiceType,
                registrationConfiguration.Context);

            return this;
        }
    }
}
