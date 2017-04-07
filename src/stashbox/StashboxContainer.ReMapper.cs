using System;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Utils;

namespace Stashbox
{
    public partial class StashboxContainer
    { 
        /// <inheritdoc />
        public IDependencyRegistrator ReMap<TFrom, TTo>(Action<IFluentServiceRegistrator> configurator)
            where TFrom : class
            where TTo : class, TFrom =>
            this.ReMap(typeof(TFrom), typeof(TTo), configurator);

        /// <inheritdoc />
        public IDependencyRegistrator ReMap<TFrom>(Type typeTo, Action<IFluentServiceRegistrator> configurator)
            where TFrom : class =>
            this.ReMap(typeof(TFrom), typeTo, configurator);

        /// <inheritdoc />
        public IDependencyRegistrator ReMap(Type typeFrom, Type typeTo, Action<IFluentServiceRegistrator> configurator)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            Shield.EnsureNotNull(configurator, nameof(configurator));

            var context = this.ServiceRegistrator.PrepareContext(typeFrom, typeTo);
            configurator(context);
            return context.ReMap();
        }

        /// <inheritdoc />
        public IDependencyRegistrator ReMap<TTo>(Action<IFluentServiceRegistrator> configurator)
             where TTo : class
        {
            var type = typeof(TTo);
            return this.ReMap(type, type, configurator);
        }

        /// <inheritdoc />
        public IDependencyRegistrator ReMap(Type typeTo, Action<IFluentServiceRegistrator> configurator) =>
            this.ReMap(typeTo, typeTo, configurator);


        /// <inheritdoc />
        public IDependencyRegistrator ReMap<TFrom, TTo>(string name = null)
            where TFrom : class
            where TTo : class, TFrom =>
            this.ReMap<TFrom, TTo>(context => context.WithName(name));

        /// <inheritdoc />
        public IDependencyRegistrator ReMap<TFrom>(Type typeTo, string name = null)
            where TFrom : class =>
            this.ReMap<TFrom>(typeTo, context => context.WithName(name));

        /// <inheritdoc />
        public IDependencyRegistrator ReMap(Type typeFrom, Type typeTo, string name = null) =>
            this.ReMap(typeFrom, typeTo, context => context.WithName(name));
    }
}
