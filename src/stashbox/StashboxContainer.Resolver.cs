using Stashbox.Utils;
using System;
using System.Collections.Generic;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public TKey Resolve<TKey>(string name = null, bool nullResultAllowed = false) where TKey : class =>
            this.ActivationContext.Activate(typeof(TKey), this, name, nullResultAllowed) as TKey;

        /// <inheritdoc />
        public object Resolve(Type typeFrom, string name = null, bool nullResultAllowed = false) =>
            this.ActivationContext.Activate(typeFrom, this, name, nullResultAllowed);

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
        public Delegate ResolveFactory(Type typeFrom, string name = null, bool nullResultAllowed = false, params Type[] parameterTypes) =>
            this.ActivationContext.ActivateFactory(typeFrom, parameterTypes, this, name, nullResultAllowed);
    }
}
