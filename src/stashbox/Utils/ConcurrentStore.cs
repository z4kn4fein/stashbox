using System.Collections;
using System.Collections.Generic;

namespace Stashbox.Utils
{
    /// <summary>
    /// Represents a concurrent collection.
    /// </summary>
    /// <typeparam name="TContent">The content type generic parameter.</typeparam>
    public class ConcurrentOrderedStore<TContent> : IEnumerable<TContent>
    {
        private ArrayStore<TContent> repository;

        /// <summary>
        /// The length of the collection.
        /// </summary>
        public int Lenght => this.repository.Length;

        /// <summary>
        /// Constructs a <see cref="ConcurrentOrderedStore{TContent}"/>
        /// </summary>
        public ConcurrentOrderedStore()
        {
            this.repository = ArrayStore<TContent>.Empty;
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="content">The item to be added.</param>
        public void Add(TContent content)
        {
            var current = this.repository;
            var newRepo = this.repository.Add(content);

            if (!Swap.TrySwapCurrent(ref this.repository, current, newRepo))
                Swap.SwapCurrent(ref this.repository, repo => repo.Add(content));
        }

        /// <summary>
        /// Gets an item from the collection.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The item.</returns>
        public TContent Get(int index) => this.repository.Get(index);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<TContent> GetEnumerator() => this.repository.GetEnumerator();
    }

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
        public KeyValuePair<TKey, TContent>[] Repository => this.repository.Repository;

        /// <summary>
        /// The last item in the collection.
        /// </summary>
        public TContent Last => this.repository.Last;

        /// <summary>
        /// Constructs a <see cref="ConcurrentOrderedStore{TContent}"/>
        /// </summary>
        public ConcurrentOrderedKeyStore()
        {
            this.repository = ArrayStoreKeyed<TKey, TContent>.Empty;
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="content">The item to be added.</param>
        public void Add(TKey key, TContent content)
        {
            var current = this.repository;
            var newRepo = this.repository.Add(key, content);

            if (!Swap.TrySwapCurrent(ref this.repository, current, newRepo))
                Swap.SwapCurrent(ref this.repository, repo => repo.Add(key, content));
        }

        /// <summary>
        /// Adds or updates an item in the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="allowUpdate">True if update is allowed, otherwise false.</param>
        public ConcurrentOrderedKeyStore<TKey, TContent> AddOrUpdate(TKey key, TContent value, bool allowUpdate = true)
        {
            var current = this.repository;
            var newRepo = this.repository.AddOrUpdate(key, value, allowUpdate);

            if (!Swap.TrySwapCurrent(ref this.repository, current, newRepo))
                Swap.SwapCurrent(ref this.repository, repo => repo.AddOrUpdate(key, value, allowUpdate));

            return this;
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
