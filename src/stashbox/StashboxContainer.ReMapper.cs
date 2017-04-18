using System;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Utils;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IDependencyRegistrator ReMap<TFrom, TTo>(Action<IFluentServiceRegistrator> configurator = null)
            where TFrom : class
            where TTo : class, TFrom =>
            this.ReMap(typeof(TFrom), typeof(TTo), configurator);

        /// <inheritdoc />
        public IDependencyRegistrator ReMap<TFrom>(Type typeTo, Action<IFluentServiceRegistrator> configurator = null)
            where TFrom : class =>
            this.ReMap(typeof(TFrom), typeTo, configurator);

        /// <inheritdoc />
        public IDependencyRegistrator ReMap(Type typeFrom, Type typeTo, Action<IFluentServiceRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var context = this.ServiceRegistrator.PrepareContext(typeFrom, typeTo);
            configurator?.Invoke(context);
            return context.ReMap();
        }

        /// <inheritdoc />
        public IDependencyRegistrator ReMap<TTo>(Action<IFluentServiceRegistrator> configurator = null)
             where TTo : class
        {
            var type = typeof(TTo);
            return this.ReMap(type, type, configurator);
        }

        /// <inheritdoc />
        public IDependencyRegistrator ReMap(Type typeTo, Action<IFluentServiceRegistrator> configurator = null) =>
            this.ReMap(typeTo, typeTo, configurator);

        /// <inheritdoc />
        public IDependencyRegistrator ReMapDecorator<TFrom, TTo>(Action<IFluentDecoratorRegistrator> configurator = null)
            where TFrom : class where TTo : class, TFrom =>
            this.ReMapDecorator(typeof(TFrom), typeof(TTo), configurator);

        /// <inheritdoc />
        public IDependencyRegistrator ReMapDecorator<TFrom>(Type typeTo, Action<IFluentDecoratorRegistrator> configurator = null)
            where TFrom : class =>
            this.ReMapDecorator(typeof(TFrom), typeTo, configurator);

        /// <inheritdoc />
        public IDependencyRegistrator ReMapDecorator(Type typeFrom, Type typeTo, Action<IFluentDecoratorRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var context = this.ServiceRegistrator.PrepareDecoratorContext(typeFrom, typeTo);
            configurator?.Invoke(context);
            return context.ReMap();
        }
    }
}
