using Stashbox.Registration.Fluent;
using Stashbox.Utils;
using System;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IStashboxContainer ReMap<TFrom, TTo>(Action<RegistrationConfigurator<TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            var registrationConfigurator = new RegistrationConfigurator<TTo>(typeof(TFrom), typeof(TTo));
            configurator?.Invoke(registrationConfigurator);
            return this.ReMapInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap<TFrom>(Type typeTo, Action<RegistrationConfigurator<TFrom>> configurator = null)
            where TFrom : class
        {
            var registrationConfigurator = new RegistrationConfigurator<TFrom>(typeof(TFrom), typeTo);
            configurator?.Invoke(registrationConfigurator);
            return this.ReMapInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap(Type typeFrom, Type typeTo, Action<RegistrationConfigurator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var registrationConfigurator = new RegistrationConfigurator(typeFrom, typeTo);
            configurator?.Invoke(registrationConfigurator);
            return this.ReMapInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap<TTo>(Action<RegistrationConfigurator<TTo>> configurator = null)
             where TTo : class
        {
            var type = typeof(TTo);
            var registrationConfigurator = new RegistrationConfigurator<TTo>(type, type);
            configurator?.Invoke(registrationConfigurator);
            return this.ReMapInternal(registrationConfigurator);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMapDecorator(Type typeFrom, Type typeTo, Action<DecoratorConfigurator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var decoratorConfigurator = new DecoratorConfigurator(typeFrom, typeTo);
            configurator?.Invoke(decoratorConfigurator);
            return this.ReMapInternal(decoratorConfigurator, true);
        }

        private IStashboxContainer ReMapInternal(RegistrationConfiguration registrationConfiguration,
            bool isDecorator = false)
        {
            this.serviceRegistrator.ReMap(
                this.registrationBuilder.BuildServiceRegistration(registrationConfiguration, isDecorator),
                registrationConfiguration.ServiceType,
                registrationConfiguration.Context);

            return this;
        }
    }
}
