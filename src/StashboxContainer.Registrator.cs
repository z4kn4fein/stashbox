using Stashbox.Exceptions;
using Stashbox.Registration.Fluent;
using Stashbox.Utils;
using System;
using Stashbox.Registration;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IStashboxContainer Register<TFrom, TTo>(Action<RegistrationConfigurator<TFrom, TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();

            var registrationConfigurator = new RegistrationConfigurator<TFrom, TTo>(typeof(TFrom), typeof(TTo));
            configurator?.Invoke(registrationConfigurator);
            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer Register<TFrom>(Type typeTo, Action<RegistrationConfigurator<TFrom, TFrom>> configurator = null)
            where TFrom : class
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var registrationConfigurator = new RegistrationConfigurator<TFrom, TFrom>(typeof(TFrom), typeTo);
            configurator?.Invoke(registrationConfigurator);

            if (!registrationConfigurator.TypeMapIsValid(out var error))
                throw new InvalidRegistrationException(registrationConfigurator.ImplementationType, error);

            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer Register(Type typeFrom, Type typeTo, Action<RegistrationConfigurator> configurator = null)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var registrationConfigurator = new RegistrationConfigurator(typeFrom, typeTo);
            configurator?.Invoke(registrationConfigurator);

            if (!registrationConfigurator.TypeMapIsValid(out var error))
                throw new InvalidRegistrationException(registrationConfigurator.ImplementationType, error);

            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer Register<TTo>(Action<RegistrationConfigurator<TTo, TTo>> configurator = null)
            where TTo : class
        {
            this.ThrowIfDisposed();
            var type = typeof(TTo);
            var registrationConfigurator = new RegistrationConfigurator<TTo, TTo>(type, type);
            configurator?.Invoke(registrationConfigurator);

            if (!registrationConfigurator.ImplementationIsResolvable(out var error))
                throw new InvalidRegistrationException(registrationConfigurator.ImplementationType, error);

            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer Register(Type typeTo, Action<RegistrationConfigurator> configurator = null) =>
            this.Register(typeTo, typeTo, configurator);

        /// <inheritdoc />
        public IStashboxContainer RegisterInstance<TInstance>(TInstance instance, object name = null, bool withoutDisposalTracking = false,
            Action<TInstance> finalizerDelegate = null) where TInstance : class
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(instance, nameof(instance));

            return this.Register<TInstance>(context =>
            {
                context.WithFinalizer(finalizerDelegate).WithInstance(instance).WithName(name);
                if (withoutDisposalTracking)
                    context.WithoutDisposalTracking();
            });
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterInstance(object instance, Type serviceType, object name = null, bool withoutDisposalTracking = false)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(instance, nameof(instance));
            Shield.EnsureNotNull(serviceType, nameof(serviceType));

            return this.Register(serviceType, context =>
            {
                context.WithInstance(instance).WithName(name);
                if (withoutDisposalTracking)
                    context.WithoutDisposalTracking();
            });
        }

        /// <inheritdoc />
        public IStashboxContainer WireUp<TInstance>(TInstance instance, object name = null, bool withoutDisposalTracking = false,
            Action<TInstance> finalizerDelegate = null) where TInstance : class
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(instance, nameof(instance));

            return this.Register<TInstance>(context =>
           {
               context.WithFinalizer(finalizerDelegate).WithInstance(instance, true).WithName(name);
               if (withoutDisposalTracking)
                   context.WithoutDisposalTracking();
           });
        }

        /// <inheritdoc />
        public IStashboxContainer WireUp(object instance, Type serviceType, object name = null, bool withoutDisposalTracking = false)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(instance, nameof(instance));
            Shield.EnsureNotNull(serviceType, nameof(serviceType));

            return this.Register(serviceType, context =>
            {
                context.WithInstance(instance, true).WithName(name);
                if (withoutDisposalTracking)
                    context.WithoutDisposalTracking();
            });
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator(Type typeFrom, Type typeTo, Action<DecoratorConfigurator> configurator = null)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var decoratorConfigurator = new DecoratorConfigurator(typeFrom, typeTo);
            configurator?.Invoke(decoratorConfigurator);

            if (!decoratorConfigurator.TypeMapIsValid(out var error))
                throw new InvalidRegistrationException(decoratorConfigurator.ImplementationType, error);

            return this.RegisterInternal(decoratorConfigurator, true);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator<TFrom, TTo>(Action<DecoratorConfigurator<TFrom, TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();
            var decoratorConfigurator = new DecoratorConfigurator<TFrom, TTo>(typeof(TFrom), typeof(TTo));
            configurator?.Invoke(decoratorConfigurator);
            return this.RegisterInternal(decoratorConfigurator, true);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator(Type typeTo, Action<DecoratorConfigurator> configurator = null)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var decoratorConfigurator = new DecoratorConfigurator(typeTo, typeTo);
            configurator?.Invoke(decoratorConfigurator);
            decoratorConfigurator.AsImplementedTypes();

            if (!decoratorConfigurator.TypeMapIsValid(out var error))
                throw new InvalidRegistrationException(decoratorConfigurator.ImplementationType, error);

            return this.RegisterInternal(decoratorConfigurator, true);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator<TTo>(Action<DecoratorConfigurator<TTo, TTo>> configurator = null)
            where TTo : class
        {
            this.ThrowIfDisposed();
            var type = typeof(TTo);
            var decoratorConfigurator = new DecoratorConfigurator<TTo, TTo>(type, type);
            configurator?.Invoke(decoratorConfigurator);
            decoratorConfigurator.AsImplementedTypes();
            return this.RegisterInternal(decoratorConfigurator, true);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator<TFrom>(Type typeTo, Action<DecoratorConfigurator<TFrom, TFrom>> configurator = null)
            where TFrom : class
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var decoratorConfigurator = new DecoratorConfigurator<TFrom, TFrom>(typeof(TFrom), typeTo);
            configurator?.Invoke(decoratorConfigurator);

            if (!decoratorConfigurator.TypeMapIsValid(out var error))
                throw new InvalidRegistrationException(decoratorConfigurator.ImplementationType, error);

            return this.RegisterInternal(decoratorConfigurator, true);
        }

        private IStashboxContainer RegisterInternal(RegistrationConfiguration registrationConfiguration, bool isDecorator = false)
        {
            ServiceRegistrator.Register(
                this.ContainerContext,
                RegistrationBuilder.BuildServiceRegistration(this.ContainerContext,
                    registrationConfiguration, isDecorator),
                registrationConfiguration.ServiceType);

            return this;
        }
    }
}
