using System;
using System.Collections.Generic;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public TKey Resolve<TKey>(string name = null, bool nullResultAllowed = false) =>
            (TKey)this.activationContext.Activate(typeof(TKey), this, name, nullResultAllowed);

        /// <inheritdoc />
        public object Resolve(Type typeFrom, string name = null, bool nullResultAllowed = false) =>
            this.activationContext.Activate(typeFrom, this, name, nullResultAllowed);

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>() =>
            (IEnumerable<TKey>)this.activationContext.Activate(typeof(IEnumerable<TKey>), this);

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom)
        {
            var type = typeof(IEnumerable<>).MakeGenericType(typeFrom);
            return (IEnumerable<object>)this.activationContext.Activate(type, this);
        }

        /// <inheritdoc />
        public Delegate ResolveFactory(Type typeFrom, string name = null, bool nullResultAllowed = false, params Type[] parameterTypes) =>
            this.activationContext.ActivateFactory(typeFrom, parameterTypes, this, name, nullResultAllowed);
    }
}
