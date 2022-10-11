using Stashbox.Registration;
using Stashbox.Registration.Fluent;
using System;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IStashboxContainer ReMap<TFrom, TTo>(Action<RegistrationConfigurator<TFrom, TTo>>? configurator = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();
            var typeFrom = typeof(TFrom);
            var registrationConfigurator = new RegistrationConfigurator<TFrom, TTo>(typeFrom, typeof(TTo), this.containerConfigurator.ContainerConfiguration.DefaultLifetime);
            configurator?.Invoke(registrationConfigurator);
            return this.ReMapInternal(typeFrom, registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap<TFrom>(Type typeTo, Action<RegistrationConfigurator<TFrom, TFrom>>? configurator = null)
            where TFrom : class
        {
            this.ThrowIfDisposed();

            var typeFrom = typeof(TFrom);
            var registrationConfigurator = new RegistrationConfigurator<TFrom, TFrom>(typeof(TFrom), typeTo, this.containerConfigurator.ContainerConfiguration.DefaultLifetime);
            configurator?.Invoke(registrationConfigurator);

            registrationConfigurator.ValidateTypeMap();

            return this.ReMapInternal(typeFrom, registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap(Type typeFrom, Type typeTo, Action<RegistrationConfigurator>? configurator = null)
        {
            this.ThrowIfDisposed();

            var registrationConfigurator = new RegistrationConfigurator(typeFrom, typeTo, this.containerConfigurator.ContainerConfiguration.DefaultLifetime);
            configurator?.Invoke(registrationConfigurator);

            registrationConfigurator.ValidateTypeMap();

            return this.ReMapInternal(typeFrom, registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap<TTo>(Action<RegistrationConfigurator<TTo, TTo>>? configurator = null)
             where TTo : class
        {
            this.ThrowIfDisposed();
            var type = typeof(TTo);
            var registrationConfigurator = new RegistrationConfigurator<TTo, TTo>(type, type, this.containerConfigurator.ContainerConfiguration.DefaultLifetime);
            configurator?.Invoke(registrationConfigurator);

            registrationConfigurator.ValidateImplementationIsResolvable();

            return this.ReMapInternal(type, registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMapDecorator(Type typeFrom, Type typeTo, Action<DecoratorConfigurator>? configurator = null)
        {
            this.ThrowIfDisposed();

            var decoratorConfigurator = new DecoratorConfigurator(typeFrom, typeTo);
            configurator?.Invoke(decoratorConfigurator);

            decoratorConfigurator.ValidateTypeMap();

            return this.ReMapInternal(typeFrom, decoratorConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMapDecorator<TFrom, TTo>(Action<DecoratorConfigurator<TFrom, TTo>>? configurator = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();
            var typeFrom = typeof(TFrom);
            var decoratorConfigurator = new DecoratorConfigurator<TFrom, TTo>(typeFrom, typeof(TTo));
            configurator?.Invoke(decoratorConfigurator);
            return this.ReMapInternal(typeFrom, decoratorConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMapDecorator<TFrom>(Type typeTo, Action<DecoratorConfigurator<TFrom, TFrom>>? configurator = null)
            where TFrom : class
        {
            this.ThrowIfDisposed();

            var typeFrom = typeof(TFrom);
            var decoratorConfigurator = new DecoratorConfigurator<TFrom, TFrom>(typeFrom, typeTo);
            configurator?.Invoke(decoratorConfigurator);

            decoratorConfigurator.ValidateTypeMap();

            return this.ReMapInternal(typeFrom, decoratorConfigurator);
        }

        private IStashboxContainer ReMapInternal(Type serviceType, ServiceRegistration serviceRegistration)
        {
            ServiceRegistrator.ReMap(
                this.ContainerContext,
                serviceRegistration,
                serviceType);

            return this;
        }
    }
}
