using Ronin.Common;
using System;

namespace Stashbox.Extensions
{
    /// <summary>
    /// Represents an extended version of the <see cref="ImmutableTree{V}"/>
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class ExtendedImmutableTree<TValue>
    {
        private ImmutableTree<TValue> repository = ImmutableTree<TValue>.Empty;
        private readonly object syncObject = new object();

        /// <summary>
        /// Gets or adds a value to the collection.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="valueFactory">The new value factory.</param>
        /// <param name="forceUpdate">If set to true a new value will be stored into the collection even if it's already exists.</param>
        /// <returns>The retrieved or added value.</returns>
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
