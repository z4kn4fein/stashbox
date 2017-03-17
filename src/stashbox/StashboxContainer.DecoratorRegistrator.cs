using System;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Registration;
using Stashbox.Utils;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IDecoratorRegistrationContext PrepareDecorator<TFrom, TTo>() where TFrom : class where TTo : class, TFrom
        {
            return new DecoratorRegistrationContext(new RegistrationContext(typeof(TFrom), typeof(TTo), this.ContainerContext, this.expressionBuilder, this.containerExtensionManager));
        }

        /// <inheritdoc />
        public IDecoratorRegistrationContext PrepareDecorator<TFrom>(Type typeTo) where TFrom : class
        {
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            return new DecoratorRegistrationContext(new RegistrationContext(typeof(TFrom), typeTo, this.ContainerContext, this.expressionBuilder, this.containerExtensionManager));
        }

        /// <inheritdoc />
        public IDecoratorRegistrationContext PrepareDecorator(Type typeFrom, Type typeTo)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            return new DecoratorRegistrationContext(new RegistrationContext(typeFrom, typeTo, this.ContainerContext, this.expressionBuilder, this.containerExtensionManager));
        }

        /// <inheritdoc />
        public IDecoratorRegistrator RegisterDecorator<TFrom, TTo>() where TFrom : class where TTo : class, TFrom
        {
            this.PrepareDecorator<TFrom, TTo>().Register();
            return this;
        }

        /// <inheritdoc />
        public IDecoratorRegistrator RegisterDecorator<TFrom>(Type typeTo) where TFrom : class
        {
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            this.PrepareDecorator<TFrom>(typeTo).Register();
            return this;
        }

        /// <inheritdoc />
        public IDecoratorRegistrator RegisterDecorator(Type typeFrom, Type typeTo)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));
            this.PrepareDecorator(typeFrom, typeTo).Register();
            return this;
        }
    }
}
