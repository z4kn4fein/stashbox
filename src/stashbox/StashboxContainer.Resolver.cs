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

        /// <inheritdoc />
        public Func<TService> ResolveFactory<TService>(string name = null, bool nullResultAllowed = false) =>
            (Func<TService>)this.ResolveFactory(typeof(TService), name, nullResultAllowed);

        /// <inheritdoc />
        public Func<T1, TService> ResolveFactory<T1, TService>(string name = null, bool nullResultAllowed = false) =>
            (Func<T1, TService>)this.ResolveFactory(typeof(TService), name, nullResultAllowed, typeof(T1));

        /// <inheritdoc />
        public Func<T1, T2, TService> ResolveFactory<T1, T2, TService>(string name = null, bool nullResultAllowed = false) =>
            (Func<T1, T2, TService>)this.ResolveFactory(typeof(TService), name, nullResultAllowed, typeof(T1), typeof(T2));

        /// <inheritdoc />
        public Func<T1, T2, T3, TService> ResolveFactory<T1, T2, T3, TService>(string name = null, bool nullResultAllowed = false) =>
            (Func<T1, T2, T3, TService>)this.ResolveFactory(typeof(TService), name, nullResultAllowed, typeof(T1), typeof(T2), typeof(T3));

        /// <inheritdoc />
        public Func<T1, T2, T3, T4, TService> ResolveFactory<T1, T2, T3, T4, TService>(string name = null, bool nullResultAllowed = false) =>
            (Func<T1, T2, T3, T4, TService>)this.ResolveFactory(typeof(TService), name, nullResultAllowed, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
    }
}
