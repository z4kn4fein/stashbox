using Stashbox.Registration.Fluent;
using Stashbox.Utils;
using System;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IStashboxContainer ReMap<TFrom, TTo>(Action<IFluentServiceConfigurator<TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            var context = new RegistrationConfigurator<TTo>(typeof(TFrom), typeof(TTo));
            configurator?.Invoke(context);
            return this.ReMap(context);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap<TFrom>(Type typeTo, Action<IFluentServiceConfigurator<TFrom>> configurator = null)
            where TFrom : class
        {
            var context = new RegistrationConfigurator<TFrom>(typeof(TFrom), typeTo);
            configurator?.Invoke(context);
            return this.ReMap(context);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap(Type typeFrom, Type typeTo, Action<IFluentServiceConfigurator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var context = new RegistrationConfigurator(typeFrom, typeTo);
            configurator?.Invoke(context);
            return this.ReMap(context);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap<TTo>(Action<IFluentServiceConfigurator<TTo>> configurator = null)
             where TTo : class
        {
            var type = typeof(TTo);
            var context = new RegistrationConfigurator<TTo>(type, type);
            configurator?.Invoke(context);
            return this.ReMap(context);
        }

        /// <inheritdoc />
        public IStashboxContainer ReMapDecorator(Type typeFrom, Type typeTo, Action<IFluentDecoratorConfigurator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var context = new DecoratorConfigurator(new RegistrationConfigurator(typeFrom, typeTo));
            configurator?.Invoke(context);
            return this.ReMap(context.RegistrationContext, true);
        }

        private IStashboxContainer ReMap(IRegistrationContext registrationContext, bool isDecorator = false) =>
            this.serviceRegistrator.ReMap(this.registrationBuilder.BuildServiceRegistration(registrationContext, isDecorator), registrationContext);
    }
}
