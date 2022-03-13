using Stashbox.Registration.Fluent;
using Stashbox.Utils;
using System;
using Stashbox.Registration;
using Stashbox.Lifetime;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IStashboxContainer Register<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();

            return this.RegisterInternal(typeof(TFrom), typeof(TTo));
        }

        /// <inheritdoc />
        public IStashboxContainer Register<TFrom, TTo>(Action<RegistrationConfigurator<TFrom, TTo>> configurator)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();

            var registrationConfigurator = new RegistrationConfigurator<TFrom, TTo>(typeof(TFrom), typeof(TTo));
            configurator(registrationConfigurator);
            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer Register<TFrom, TTo>(object name)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();

            return this.RegisterInternal(typeof(TFrom), typeof(TTo), name);
        }

        /// <inheritdoc />
        public IStashboxContainer Register<TFrom>(Type typeTo)
            where TFrom : class
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var typeFrom = typeof(TFrom);

            Shield.EnsureTypeMapIsValid(typeFrom, typeTo);

            return this.RegisterInternal(typeof(TFrom), typeTo);
        }

        /// <inheritdoc />
        public IStashboxContainer Register<TFrom>(Type typeTo, Action<RegistrationConfigurator<TFrom, TFrom>> configurator)
            where TFrom : class
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var registrationConfigurator = new RegistrationConfigurator<TFrom, TFrom>(typeof(TFrom), typeTo);
            configurator(registrationConfigurator);

            registrationConfigurator.ValidateTypeMap();
            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer Register(Type typeFrom, Type typeTo)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            Shield.EnsureTypeMapIsValid(typeFrom, typeTo);

            return this.RegisterInternal(typeFrom, typeTo);
        }

        /// <inheritdoc />
        public IStashboxContainer Register(Type typeFrom, Type typeTo, Action<RegistrationConfigurator> configurator)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var registrationConfigurator = new RegistrationConfigurator(typeFrom, typeTo);
            configurator(registrationConfigurator);

            registrationConfigurator.ValidateTypeMap();

            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer Register<TTo>()
            where TTo : class
        {
            this.ThrowIfDisposed();

            var type = typeof(TTo);
            Shield.EnsureIsResolvable(type);

            return this.RegisterInternal(type, type);
        }

        /// <inheritdoc />
        public IStashboxContainer Register<TTo>(Action<RegistrationConfigurator<TTo, TTo>> configurator)
            where TTo : class
        {
            this.ThrowIfDisposed();
            var type = typeof(TTo);
            var registrationConfigurator = new RegistrationConfigurator<TTo, TTo>(type, type);
            configurator(registrationConfigurator);

            registrationConfigurator.ValidateImplementationIsResolvable();

            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer Register(Type typeTo)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            Shield.EnsureIsResolvable(typeTo);

            return this.RegisterInternal(typeTo, typeTo);
        }

        /// <inheritdoc />
        public IStashboxContainer Register(Type typeTo, Action<RegistrationConfigurator> configurator)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var registrationConfigurator = new RegistrationConfigurator(typeTo, typeTo);
            configurator(registrationConfigurator);

            registrationConfigurator.ValidateImplementationIsResolvable();

            return this.RegisterInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterSingleton<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();

            return this.RegisterInternal(typeof(TFrom), typeof(TTo), lifetime: Lifetimes.Singleton);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterSingleton<TFrom, TTo>(object name)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();

            return this.RegisterInternal(typeof(TFrom), typeof(TTo), name, lifetime: Lifetimes.Singleton);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterSingleton<TTo>()
            where TTo : class
        {
            this.ThrowIfDisposed();

            var type = typeof(TTo);
            Shield.EnsureIsResolvable(type);

            return this.RegisterInternal(type, type, lifetime: Lifetimes.Singleton);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterSingleton<TTo>(object name)
            where TTo : class
        {
            this.ThrowIfDisposed();

            var type = typeof(TTo);
            Shield.EnsureIsResolvable(type);

            return this.RegisterInternal(type, type, name, lifetime: Lifetimes.Singleton);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterSingleton(Type typeFrom, Type typeTo)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            Shield.EnsureTypeMapIsValid(typeFrom, typeTo);

            return this.RegisterInternal(typeFrom, typeTo, lifetime: Lifetimes.Singleton);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterSingleton(Type typeFrom, Type typeTo, object name)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            Shield.EnsureTypeMapIsValid(typeFrom, typeTo);

            return this.RegisterInternal(typeFrom, typeTo, name, lifetime: Lifetimes.Singleton);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterScoped<TFrom, TTo>()
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();

            return this.RegisterInternal(typeof(TFrom), typeof(TTo), lifetime: Lifetimes.Scoped);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterScoped<TFrom, TTo>(object name)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();

            return this.RegisterInternal(typeof(TFrom), typeof(TTo), name, lifetime: Lifetimes.Scoped);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterScoped(Type typeFrom, Type typeTo)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            Shield.EnsureTypeMapIsValid(typeFrom, typeTo);

            return this.RegisterInternal(typeFrom, typeTo, lifetime: Lifetimes.Scoped);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterScoped(Type typeFrom, Type typeTo, object name)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            Shield.EnsureTypeMapIsValid(typeFrom, typeTo);

            return this.RegisterInternal(typeFrom, typeTo, name, lifetime: Lifetimes.Scoped);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterScoped<TTo>()
            where TTo : class
        {
            this.ThrowIfDisposed();

            var type = typeof(TTo);
            Shield.EnsureIsResolvable(type);

            return this.RegisterInternal(type, type, lifetime: Lifetimes.Scoped);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterScoped<TTo>(object name)
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
        public IStashboxContainer RegisterDecorator(Type typeFrom, Type typeTo)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            Shield.EnsureTypeMapIsValid(typeFrom, typeTo);

            return this.RegisterInternal(typeFrom, typeTo, lifetime: Lifetimes.Empty, isDecorator: true);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator(Type typeFrom, Type typeTo, Action<DecoratorConfigurator> configurator)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var decoratorConfigurator = new DecoratorConfigurator(typeFrom, typeTo);
            configurator(decoratorConfigurator);

            decoratorConfigurator.ValidateTypeMap();

            return this.RegisterInternal(decoratorConfigurator, true);
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
        public IStashboxContainer RegisterDecorator<TFrom, TTo>(Action<DecoratorConfigurator<TFrom, TTo>> configurator)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();
            var decoratorConfigurator = new DecoratorConfigurator<TFrom, TTo>(typeof(TFrom), typeof(TTo));
            configurator(decoratorConfigurator);
            return this.RegisterInternal(decoratorConfigurator, true);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator(Type typeTo)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var decoratorConfigurator = new DecoratorConfigurator(typeTo, typeTo);
            decoratorConfigurator
                .AsImplementedTypes()
                .ValidateTypeMap();

            return this.RegisterInternal(decoratorConfigurator, true);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator(Type typeTo, Action<DecoratorConfigurator> configurator)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var decoratorConfigurator = new DecoratorConfigurator(typeTo, typeTo);
            configurator(decoratorConfigurator);
            decoratorConfigurator
                .AsImplementedTypes()
                .ValidateTypeMap();

            return this.RegisterInternal(decoratorConfigurator, true);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator<TTo>()
            where TTo : class
        {
            this.ThrowIfDisposed();

            var type = typeof(TTo);

            var decoratorConfigurator = new DecoratorConfigurator<TTo, TTo>(type, type);
            decoratorConfigurator
                .AsImplementedTypes()
                .ValidateImplementationIsResolvable();

            return this.RegisterInternal(decoratorConfigurator, true);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator<TTo>(Action<DecoratorConfigurator<TTo, TTo>> configurator)
            where TTo : class
        {
            this.ThrowIfDisposed();
            var type = typeof(TTo);

            var decoratorConfigurator = new DecoratorConfigurator<TTo, TTo>(type, type);
            configurator(decoratorConfigurator);
            decoratorConfigurator
                .AsImplementedTypes()
                .ValidateImplementationIsResolvable();

            return this.RegisterInternal(decoratorConfigurator, true);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator<TFrom>(Type typeTo)
            where TFrom : class
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var typeFrom = typeof(TFrom);

            Shield.EnsureTypeMapIsValid(typeFrom, typeTo);

            return this.RegisterInternal(typeFrom, typeTo, lifetime: Lifetimes.Empty, isDecorator: true);
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator<TFrom>(Type typeTo, Action<DecoratorConfigurator<TFrom, TFrom>> configurator)
            where TFrom : class
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var decoratorConfigurator = new DecoratorConfigurator<TFrom, TFrom>(typeof(TFrom), typeTo);
            configurator(decoratorConfigurator);

            decoratorConfigurator.ValidateTypeMap();

            return this.RegisterInternal(decoratorConfigurator, true);
        }

        private IStashboxContainer RegisterInternal(Type serviceType, Type implementationType, object? name = null,
            LifetimeDescriptor? lifetime = null, bool isDecorator = false)
        {
            ServiceRegistrator.Register(
                this.ContainerContext,
                RegistrationBuilder.BuildServiceRegistration(this.ContainerContext,
                    implementationType, name, lifetime, isDecorator),
                serviceType);

            return this;
        }

        private IStashboxContainer RegisterInstanceInternal(Type serviceType, Type implementationType, object instance, bool isWireUp,
            bool withoutDisposalTracking, object? name, Action<object>? finalizer = null)
        {
            ServiceRegistrator.Register(
                this.ContainerContext,
                RegistrationBuilder.BuildInstanceRegistration(this.ContainerContext,
                    implementationType, name, instance, isWireUp, withoutDisposalTracking, finalizer),
                serviceType);

            return this;
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
