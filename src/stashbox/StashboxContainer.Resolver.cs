using Stashbox.Utils;
using System;
using System.Collections.Generic;
using Stashbox.Infrastructure;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public TKey Resolve<TKey>(string name = null) where TKey : class =>
            this.ActivationContext.Activate(typeof(TKey), this, name) as TKey;

        /// <inheritdoc />
        public object Resolve(Type typeFrom, string name = null) =>
            this.ActivationContext.Activate(typeFrom, this, name);

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>() where TKey : class =>
            this.ActivationContext.Activate(typeof(IEnumerable<TKey>), this) as IEnumerable<TKey>;

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            var type = typeof(IEnumerable<>).MakeGenericType(typeFrom);
            return (IEnumerable<object>)this.ActivationContext.Activate(type, this);
        }

        /// <inheritdoc />
        public Delegate ResolveFactory(Type typeFrom, string name = null, params Type[] parameterTypes) =>
            this.ActivationContext.ActivateFactory(typeFrom, parameterTypes, this, name);
    }
}
