using Stashbox.Registration;
using Stashbox.Utils;
using System;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IStashboxContainer ReMap<TFrom, TTo>(Action<IFluentServiceRegistrator<TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            var context = this.ServiceRegistrator.PrepareContext<TTo>(typeof(TFrom), typeof(TTo));
            configurator?.Invoke(context);
            return context.ReMap();
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap<TFrom>(Type typeTo, Action<IFluentServiceRegistrator<TFrom>> configurator = null)
            where TFrom : class
        {
            var context = this.ServiceRegistrator.PrepareContext<TFrom>(typeof(TFrom), typeTo);
            configurator?.Invoke(context);
            return context.ReMap();
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap(Type typeFrom, Type typeTo, Action<IFluentServiceRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var context = this.ServiceRegistrator.PrepareContext(typeFrom, typeTo);
            configurator?.Invoke(context);
            return context.ReMap();
        }

        /// <inheritdoc />
        public IStashboxContainer ReMap<TTo>(Action<IFluentServiceRegistrator<TTo>> configurator = null)
             where TTo : class
        {
            var type = typeof(TTo);
            var context = this.ServiceRegistrator.PrepareContext<TTo>(type, type);
            configurator?.Invoke(context);
            return context.ReMap();
        }

        /// <inheritdoc />
        public IStashboxContainer ReMapDecorator(Type typeFrom, Type typeTo, Action<IFluentDecoratorRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var context = this.ServiceRegistrator.PrepareDecoratorContext(typeFrom, typeTo);
            configurator?.Invoke(context);
            return context.ReMap();
        }
    }
}
