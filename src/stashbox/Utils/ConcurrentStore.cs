using Stashbox.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.Utils
{
    /// <summary>
    /// Represents a concurrent collection.
    /// </summary>
    /// <typeparam name="TContent">The content type generic parameter.</typeparam>
    /// <typeparam name="TKey">The key.</typeparam>
    public class ConcurrentOrderedKeyStore<TKey, TContent> : IEnumerable<TContent>
    {
        private ArrayStoreKeyed<TKey, TContent> repository;

        /// <summary>
        /// The length of the collection.
        /// </summary>
        public int Lenght => this.repository.Length;

        /// <summary>
        /// The array of the items.
        /// </summary>
        public KeyValue<TKey, TContent>[] Repository => this.repository.Repository;

        /// <summary>
        /// The last item in the collection.
        /// </summary>
        public TContent Last => this.repository.Last;

        /// <summary>
        /// Constructs a <see cref="ConcurrentOrderedKeyStore{TKey,TContent}"/>
        /// </summary>
        public ConcurrentOrderedKeyStore()
        {
            this.repository = ArrayStoreKeyed<TKey, TContent>.Empty;
        }

        private ConcurrentOrderedKeyStore(KeyValue<TKey, TContent>[] initial)
        {
            this.repository = new ArrayStoreKeyed<TKey, TContent>(initial);
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="content">The item to be added.</param>
        public void Add(TKey key, TContent content) =>
            Swap.SwapValue(ref this.repository, repo => repo.Add(key, content));

        /// <summary>
        /// Adds or updates an item in the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="allowUpdate">True if update is allowed, otherwise false.</param>
        public ConcurrentOrderedKeyStore<TKey, TContent> AddOrUpdate(TKey key, TContent value, bool allowUpdate = true)
        {
            Swap.SwapValue(ref this.repository, repo => repo.AddOrUpdate(key, value));
            return this;
        }

        internal ConcurrentOrderedKeyStore<TKey, TContent> WhereOrDefault(Func<KeyValue<TKey, TContent>, bool> predicate)
        {
            var initial = this.repository.Repository.Where(predicate).ToArray();
            if (initial.Length == 0)
                return null;

            return new ConcurrentOrderedKeyStore<TKey, TContent>(initial);
        }

        /// <summary>
        /// Gets an item from the collection.
        /// </summary>
        /// <param name="key">The key of the item.</param>
        /// <returns>The item.</returns>
        public TContent GetOrDefault(TKey key) => this.repository.GetOrDefault(key);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<TContent> GetEnumerator() =>
            this.repository.GetEnumerator();
    }
}
