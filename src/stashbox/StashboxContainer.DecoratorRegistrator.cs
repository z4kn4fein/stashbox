using System;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Utils;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IDecoratorRegistrator RegisterDecorator<TFrom, TTo>(Action<IFluentDecoratorRegistrator> configurator = null) where TFrom : class where TTo : class, TFrom =>
            this.RegisterDecorator(typeof(TFrom), typeof(TTo), configurator);

        /// <inheritdoc />
        public IDecoratorRegistrator RegisterDecorator<TFrom>(Type typeTo, Action<IFluentDecoratorRegistrator> configurator = null) where TFrom : class =>
            this.RegisterDecorator(typeof(TFrom), typeTo, configurator);

        /// <inheritdoc />
        public IDecoratorRegistrator RegisterDecorator(Type typeFrom, Type typeTo, Action<IFluentDecoratorRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var context = this.ServiceRegistrator.PrepareDecoratorContext(typeFrom, typeTo);
            configurator?.Invoke(context);
            return context.Register();
        }
    }
}
