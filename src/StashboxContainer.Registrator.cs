using Stashbox.Lifetime;
using Stashbox.Registration;
using Stashbox.Registration.Fluent;
using Stashbox.Registration.ServiceRegistrations;
using Stashbox.Utils;
using System;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IStashboxContainer Register<TFrom, TTo>(Action<RegistrationConfigurator<TFrom, TTo>> configurator)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();

            var registrationConfigurator = new RegistrationConfigurator<TFrom, TTo>(typeof(TFrom), typeof(TTo), this.containerConfigurator.ContainerConfiguration.DefaultLifetime);
            configurator(registrationConfigurator);
            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer Register<TFrom, TTo>(object? name = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();

            return this.RegisterInternal(typeof(TFrom), typeof(TTo), name);
        }

        /// <inheritdoc />
        public IStashboxContainer Register<TFrom>(Type typeTo, Action<RegistrationConfigurator<TFrom, TFrom>>? configurator = null)
            where TFrom : class
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var typeFrom = typeof(TFrom);

            if (configurator == null)
            {
                Shield.EnsureTypeMapIsValid(typeFrom, typeTo);
                return this.RegisterInternal(typeFrom, typeTo);
            }

            var registrationConfigurator = new RegistrationConfigurator<TFrom, TFrom>(typeFrom, typeTo, this.containerConfigurator.ContainerConfiguration.DefaultLifetime);
            configurator(registrationConfigurator);

            registrationConfigurator.ValidateTypeMap();
            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer Register(Type typeFrom, Type typeTo, Action<RegistrationConfigurator>? configurator = null)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            if (configurator == null)
            {
                Shield.EnsureTypeMapIsValid(typeFrom, typeTo);
                return this.RegisterInternal(typeFrom, typeTo);
            }

            var registrationConfigurator = new RegistrationConfigurator(typeFrom, typeTo, this.containerConfigurator.ContainerConfiguration.DefaultLifetime);
            configurator(registrationConfigurator);

            registrationConfigurator.ValidateTypeMap();
            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer Register<TTo>(Action<RegistrationConfigurator<TTo, TTo>> configurator)
            where TTo : class
        {
            this.ThrowIfDisposed();
            var type = typeof(TTo);

            var registrationConfigurator = new RegistrationConfigurator<TTo, TTo>(type, type, this.containerConfigurator.ContainerConfiguration.DefaultLifetime);
            configurator(registrationConfigurator);

            registrationConfigurator.ValidateImplementationIsResolvable();
            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer Register<TTo>(object? name = null)
            where TTo : class
        {
            this.ThrowIfDisposed();

            var type = typeof(TTo);
            Shield.EnsureIsResolvable(type);

            return this.RegisterInternal(type, type, name);
        }

        /// <inheritdoc />
        public IStashboxContainer Register(Type typeTo, Action<RegistrationConfigurator>? configurator = null)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            if (configurator == null)
            {
                Shield.EnsureIsResolvable(typeTo);
                return this.RegisterInternal(typeTo, typeTo);
            }

            var registrationConfigurator = new RegistrationConfigurator(typeTo, typeTo, this.containerConfigurator.ContainerConfiguration.DefaultLifetime);
            configurator(registrationConfigurator);

            registrationConfigurator.ValidateImplementationIsResolvable();

            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterSingleton<TFrom, TTo>(object? name = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();

            return this.RegisterInternal(typeof(TFrom), typeof(TTo), name, lifetime: Lifetimes.Singleton);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterSingleton<TTo>(object? name = null)
            where TTo : class
        {
            this.ThrowIfDisposed();

            var type = typeof(TTo);
            Shield.EnsureIsResolvable(type);

            return this.RegisterInternal(type, type, name, lifetime: Lifetimes.Singleton);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterSingleton(Type typeFrom, Type typeTo, object? name = null)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            Shield.EnsureTypeMapIsValid(typeFrom, typeTo);

            return this.RegisterInternal(typeFrom, typeTo, name, lifetime: Lifetimes.Singleton);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterScoped<TFrom, TTo>(object? name = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();

            return this.RegisterInternal(typeof(TFrom), typeof(TTo), name, lifetime: Lifetimes.Scoped);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterScoped(Type typeFrom, Type typeTo, object? name = null)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            Shield.EnsureTypeMapIsValid(typeFrom, typeTo);

            return this.RegisterInternal(typeFrom, typeTo, name, lifetime: Lifetimes.Scoped);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterScoped<TTo>(object? name = null)
            where TTo : class
        {
            this.ThrowIfDisposed();

            var type = typeof(TTo);
            Shield.EnsureIsResolvable(type);

            return this.RegisterInternal(type, type, name, lifetime: Lifetimes.Scoped);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterInstance<TInstance>(TInstance instance, object? name = null, bool withoutDisposalTracking = false,
            Action<TInstance>? finalizerDelegate = null) where TInstance : class
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(instance, nameof(instance));

            return this.RegisterInstanceInternal(typeof(TInstance), instance.GetType(), instance, false, withoutDisposalTracking, name, finalizerDelegate != null
                ? o => finalizerDelegate((TInstance)o)
                : null);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterInstance(object instance, Type serviceType, object? name = null, bool withoutDisposalTracking = false)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(instance, nameof(instance));
            Shield.EnsureNotNull(serviceType, nameof(serviceType));

            return this.RegisterInstanceInternal(serviceType, instance.GetType(), instance, false, withoutDisposalTracking, name);
        }

        /// <inheritdoc />
        public IStashboxContainer WireUp<TInstance>(TInstance instance, object? name = null, bool withoutDisposalTracking = false,
            Action<TInstance>? finalizerDelegate = null) where TInstance : class
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(instance, nameof(instance));

            return this.RegisterInstanceInternal(typeof(TInstance), instance.GetType(), instance, true, withoutDisposalTracking, name, finalizerDelegate != null
                 ? o => finalizerDelegate((TInstance)o)
                 : null);
        }

        /// <inheritdoc />
        public IStashboxContainer WireUp(object instance, Type serviceType, object? name = null, bool withoutDisposalTracking = false)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(instance, nameof(instance));
            Shield.EnsureNotNull(serviceType, nameof(serviceType));

            return this.RegisterInstanceInternal(serviceType, instance.GetType(), instance, true, withoutDisposalTracking, name);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator(Type typeFrom, Type typeTo, Action<DecoratorConfigurator>? configurator = null)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            if (configurator == null)
            {
                Shield.EnsureTypeMapIsValid(typeFrom, typeTo);
                return this.RegisterInternal(typeFrom, typeTo, lifetime: Lifetimes.Empty, isDecorator: true);
            }

            var decoratorConfigurator = new DecoratorConfigurator(typeFrom, typeTo);
            configurator(decoratorConfigurator);

            decoratorConfigurator.ValidateTypeMap();

            return this.RegisterInternal(decoratorConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();

            return this.RegisterInternal(typeof(TFrom), typeof(TTo), lifetime: Lifetimes.Empty, isDecorator: true);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator<TFrom, TTo>(Action<DecoratorConfigurator<TFrom, TTo>>? configurator = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();

            if (configurator == null) return this.RegisterInternal(typeof(TFrom), typeof(TTo), lifetime: Lifetimes.Empty, isDecorator: true);

            var decoratorConfigurator = new DecoratorConfigurator<TFrom, TTo>(typeof(TFrom), typeof(TTo));
            configurator(decoratorConfigurator);
            return this.RegisterInternal(decoratorConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator(Type typeTo, Action<DecoratorConfigurator>? configurator = null)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var decoratorConfigurator = new DecoratorConfigurator(typeTo, typeTo);
            configurator?.Invoke(decoratorConfigurator);
            decoratorConfigurator
                .AsImplementedTypes()
                .ValidateTypeMap();

            return this.RegisterInternal(decoratorConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator<TTo>(Action<DecoratorConfigurator<TTo, TTo>>? configurator = null)
            where TTo : class
        {
            this.ThrowIfDisposed();
            var type = typeof(TTo);

            var decoratorConfigurator = new DecoratorConfigurator<TTo, TTo>(type, type);
            configurator?.Invoke(decoratorConfigurator);
            decoratorConfigurator
                .AsImplementedTypes()
                .ValidateImplementationIsResolvable();

            return this.RegisterInternal(decoratorConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator<TFrom>(Type typeTo, Action<DecoratorConfigurator<TFrom, TFrom>>? configurator = null)
            where TFrom : class
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var typeFrom = typeof(TFrom);

            if (configurator == null)
            {
                Shield.EnsureTypeMapIsValid(typeFrom, typeTo);
                return this.RegisterInternal(typeFrom, typeTo, lifetime: Lifetimes.Empty, isDecorator: true);
            }

            var decoratorConfigurator = new DecoratorConfigurator<TFrom, TFrom>(typeof(TFrom), typeTo);
            configurator(decoratorConfigurator);

            decoratorConfigurator.ValidateTypeMap();

            return this.RegisterInternal(decoratorConfigurator);
        }

        private IStashboxContainer RegisterInternal(Type serviceType, Type implementationType, object? name = null,
            LifetimeDescriptor? lifetime = null, bool isDecorator = false)
        {
            ServiceRegistrator.Register(
                this.ContainerContext,
                new ServiceRegistration(implementationType, name, lifetime ?? this.ContainerContext.ContainerConfiguration.DefaultLifetime, isDecorator),
                serviceType);

            return this;
        }

        private IStashboxContainer RegisterInternal(RegistrationConfiguration configuration)
        {
            ServiceRegistrator.Register(
                this.ContainerContext,
                configuration.Registration,
                configuration.ServiceType);

            return this;
        }

        private IStashboxContainer RegisterInstanceInternal(Type serviceType, Type implementationType, object instance, bool isWireUp,
            bool withoutDisposalTracking, object? name, Action<object>? finalizer = null)
        {
            ServiceRegistrator.Register(
                this.ContainerContext,
                new InstanceRegistration(instance, isWireUp, new ServiceRegistration(implementationType, name, Lifetimes.Empty, false))
                {
                    Finalizer = finalizer,
                    IsLifetimeExternallyOwned = withoutDisposalTracking,
                },
                serviceType);

            return this;
        }
    }
}
