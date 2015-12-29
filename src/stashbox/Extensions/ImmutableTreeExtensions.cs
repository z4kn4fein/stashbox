using Ronin.Common;
using System;

namespace Stashbox.Extensions
{
    public class ExtendedImmutableTree<TValue>
    {
        private ImmutableTree<TValue> repository = ImmutableTree<TValue>.Empty;
        private readonly object syncObject = new object();


        public TValue GetOrAdd<TKey>(TKey key, Func<TValue> valueFactory, bool forceUpdate = false)
        {
            var hash = key.GetHashCode();
            TValue value;
            if (!forceUpdate)
            {
                value = this.repository.GetValueOrDefault(hash);
                if (value != null) return value;
            }

            lock (this.syncObject)
            {
                value = valueFactory();
                this.repository = this.repository.AddOrUpdate(hash, value, (oldValue, newValue) => value);
            }

            return value;
        }
    }
}
