using Stashbox.Registration;
using Stashbox.Utils;
using System;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator(Type typeFrom, Type typeTo, Action<IFluentDecoratorRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var context = this.ServiceRegistrator.PrepareDecoratorContext(typeFrom, typeTo);
            configurator?.Invoke(context);
            return context.Register();
        }
    }
}
