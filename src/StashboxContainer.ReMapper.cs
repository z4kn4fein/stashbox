using Stashbox.Exceptions;
using Stashbox.Registration.Fluent;
using Stashbox.Utils;
using System;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IStashboxContainer ReMap<TFrom, TTo>(Action<RegistrationConfigurator<TFrom, TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();
            var registrationConfigurator = new RegistrationConfigurator<TFrom, TTo>(typeof(TFrom), typeof(TTo));
            configurator?.Invoke(registrationConfigurator);
            return this.ReMapInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap<TFrom>(Type typeTo, Action<RegistrationConfigurator<TFrom, TFrom>> configurator = null)
            where TFrom : class
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var registrationConfigurator = new RegistrationConfigurator<TFrom, TFrom>(typeof(TFrom), typeTo);
            configurator?.Invoke(registrationConfigurator);

            if (!registrationConfigurator.TypeMapIsValid(out var error))
                throw new InvalidRegistrationException(registrationConfigurator.ImplementationType, error);

            return this.ReMapInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap(Type typeFrom, Type typeTo, Action<RegistrationConfigurator> configurator = null)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var registrationConfigurator = new RegistrationConfigurator(typeFrom, typeTo);
            configurator?.Invoke(registrationConfigurator);

            if (!registrationConfigurator.TypeMapIsValid(out var error))
                throw new InvalidRegistrationException(registrationConfigurator.ImplementationType, error);

            return this.ReMapInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap<TTo>(Action<RegistrationConfigurator<TTo, TTo>> configurator = null)
             where TTo : class
        {
            this.ThrowIfDisposed();
            var type = typeof(TTo);
            var registrationConfigurator = new RegistrationConfigurator<TTo, TTo>(type, type);
            configurator?.Invoke(registrationConfigurator);

            if (!registrationConfigurator.ImplementationIsResolvable(out var error))
                throw new InvalidRegistrationException(registrationConfigurator.ImplementationType, error);

            return this.ReMapInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMapDecorator(Type typeFrom, Type typeTo, Action<DecoratorConfigurator> configurator = null)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var decoratorConfigurator = new DecoratorConfigurator(typeFrom, typeTo);
            configurator?.Invoke(decoratorConfigurator);

            if (!decoratorConfigurator.TypeMapIsValid(out var error))
                throw new InvalidRegistrationException(decoratorConfigurator.ImplementationType, error);

            return this.ReMapInternal(decoratorConfigurator, true);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMapDecorator<TFrom, TTo>(Action<DecoratorConfigurator<TFrom, TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            this.ThrowIfDisposed();
            var decoratorConfigurator = new DecoratorConfigurator<TFrom, TTo>(typeof(TFrom), typeof(TTo));
            configurator?.Invoke(decoratorConfigurator);
            return this.ReMapInternal(decoratorConfigurator, true);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMapDecorator<TFrom>(Type typeTo, Action<DecoratorConfigurator<TFrom, TFrom>> configurator = null)
            where TFrom : class
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var registrationConfigurator = new DecoratorConfigurator<TFrom, TFrom>(typeof(TFrom), typeTo);
            configurator?.Invoke(registrationConfigurator);

            if (!registrationConfigurator.TypeMapIsValid(out var error))
                throw new InvalidRegistrationException(registrationConfigurator.ImplementationType, error);

            return this.ReMapInternal(registrationConfigurator, true);
        }

        private IStashboxContainer ReMapInternal(RegistrationConfiguration registrationConfiguration,
            bool isDecorator = false)
        {
            this.serviceRegistrator.ReMap(
                this.ContainerContext,
                this.registrationBuilder.BuildServiceRegistration(this.ContainerContext,
                    registrationConfiguration, isDecorator),
                registrationConfiguration.ServiceType);

            return this;
        }
    }
}
