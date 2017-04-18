using System;
using System.Collections;
using System.Collections.Generic;

namespace Stashbox.Utils
{
    /// <summary>
    /// Represents an immutable AVL tree
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class ConcurrentTree<TKey, TValue> : IEnumerable<TValue>
    {
        /// <summary>
        /// Static factory method.
        /// </summary>
        /// <returns>A new tree instance</returns>
        public static ConcurrentTree<TKey, TValue> Create() => new ConcurrentTree<TKey, TValue>();

        private readonly Swap<AvlTreeKeyValue<TKey, TValue>> repository;

        /// <summary>
        /// The current root value of the tree,
        /// </summary>
        public TValue Value => this.repository.Value.Value;

        /// <summary>
        /// Inidicates that the tree has more nodes than the root one.
        /// </summary>
        public bool HasMultipleItems => this.repository.Value.HasMultipleItems;

        /// <summary>
        /// True if the tree is empty, otherwise false.
        /// </summary>
        public bool IsEmpty => this.repository.Value.IsEmpty;

        /// <summary>
        /// Constructs the <see cref="ConcurrentTree{TKey, TValue}"/>
        /// </summary>
        public ConcurrentTree()
        {
            this.repository = new Swap<AvlTreeKeyValue<TKey, TValue>>(AvlTreeKeyValue<TKey, TValue>.Empty);
        }

        /// <summary>
        /// Returns with the value specified by the given key if it's exist, otherwise it's default value will be returned.
        /// </summary>
        /// <param name="key">The key of the entry.</param>
        /// <returns>The found or the default value.</returns>
        public TValue GetOrDefault(TKey key) =>
            this.repository.Value.GetOrDefault(key);

        /// <summary>
        /// Returns with the value specified by the given key if it's exist, otherwise it's default value will be returned.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="key">The key of the entry.</param>
        /// <returns>The found or the default value.</returns>
        public TValue GetOrDefault(int hash, TKey key) =>
            this.repository.Value.GetOrDefault(hash, key);

        /// <summary>
        /// Inserts an item into the tree if it doesn't exist, otherwise the existing item will be replaced if the update delegate is set.
        /// </summary>
        /// <param name="key">The key of the entry.</param>
        /// <param name="value">The value which will be inserted if it doesn't exist yet.</param>
        /// <param name="updateDelegate">The update delegate which will be invoked when the value is already stored on the tree.</param>
        /// <returns>The modified tree.</returns>
        public ConcurrentTree<TKey, TValue> AddOrUpdate(TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null) =>
            this.AddOrUpdate(key.GetHashCode(), key, value, updateDelegate);

        /// <summary>
        /// Inserts an item into the tree if it doesn't exist, otherwise the existing item will be replaced if the update delegate is set.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="key">The key of the entry.</param>
        /// <param name="value">The value which will be inserted if it doesn't exist yet.</param>
        /// <param name="updateDelegate">The update delegate which will be invoked when the value is already stored on the tree.</param>
        /// <returns>The modified tree.</returns>
        public ConcurrentTree<TKey, TValue> AddOrUpdate(int hash, TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null)
        {
            var currentRepo = this.repository.Value;
            var newRepo = this.repository.Value.AddOrUpdate(hash, key, value, updateDelegate);

            if (!this.repository.TrySwapCurrent(currentRepo, newRepo))
                this.repository.SwapCurrent(repo => repo.AddOrUpdate(hash, key, value, updateDelegate));

            return this;
        }

        /// <inheritdoc />
        public IEnumerator<TValue> GetEnumerator() => this.repository.Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.repository.Value.GetEnumerator();
    }

    /// <summary>
    /// Represents an immutable AVL tree
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class ConcurrentTree<TValue> : IEnumerable<TValue>
    {
        /// <summary>
        /// Static factory method.
        /// </summary>
        /// <returns>A new tree instance</returns>
        public static ConcurrentTree<TValue> Create() => new ConcurrentTree<TValue>();

        private readonly Swap<AvlTree<TValue>> repository;

        /// <summary>
        /// The current root value of the tree,
        /// </summary>
        public TValue Value => this.repository.Value.Value;

        /// <summary>
        /// Inidicates that the tree has more nodes than the root one.
        /// </summary>
        public bool HasMultipleItems => this.repository.Value.HasMultipleItems;

        /// <summary>
        /// True if the tree is empty, otherwise false.
        /// </summary>
        public bool IsEmpty => this.repository.Value.IsEmpty;

        /// <summary>
        /// Constructs the <see cref="ConcurrentTree{TKey, TValue}"/>
        /// </summary>
        public ConcurrentTree()
        {
            this.repository = new Swap<AvlTree<TValue>>(AvlTree<TValue>.Empty);
        }

        /// <summary>
        /// Returns with the value specified by the given key if it's exist, otherwise it's default value will be returned.
        /// </summary>
        /// <param name="key">The key of the entry.</param>
        /// <returns>The found or the default value.</returns>
        public TValue GetOrDefault(int key) =>
            this.repository.Value.GetOrDefault(key);


        /// <summary>
        /// Inserts an item into the tree if it doesn't exist, otherwise the existing item will be replaced if the update delegate is set.
        /// </summary>
        /// <param name="key">The key of the entry.</param>
        /// <param name="value">The value which will be inserted if it doesn't exist yet.</param>
        /// <param name="updateDelegate">The update delegate which will be invoked when the value is already stored on the tree.</param>
        /// <returns>The modified tree.</returns>
        public ConcurrentTree<TValue> AddOrUpdate(int key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null)
        {
            var currentRepo = this.repository.Value;
            var newRepo = this.repository.Value.AddOrUpdate(key, value, updateDelegate);

            if (!this.repository.TrySwapCurrent(currentRepo, newRepo))
                this.repository.SwapCurrent(repo => repo.AddOrUpdate(key, value, updateDelegate));

            return this;
        }

        /// <inheritdoc />
        public IEnumerator<TValue> GetEnumerator() => this.repository.Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.repository.Value.GetEnumerator();
    }
}
