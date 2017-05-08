using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    /// <summary>
    /// Represents concurrent AVL tree
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

        private AvlTreeKeyValue<TKey, TValue> repository;

        /// <summary>
        /// The current root value of the tree,
        /// </summary>
        public TValue Value => this.repository.Value;

        /// <summary>
        /// Inidicates that the tree has more nodes than the root one.
        /// </summary>
        public bool HasMultipleItems => this.repository.HasMultipleItems;

        /// <summary>
        /// True if the tree is empty, otherwise false.
        /// </summary>
        public bool IsEmpty => this.repository.IsEmpty;

        /// <summary>
        /// Constructs the <see cref="ConcurrentTree{TKey, TValue}"/>
        /// </summary>
        public ConcurrentTree()
        {
            this.repository = AvlTreeKeyValue<TKey, TValue>.Empty;
        }

        /// <summary>
        /// Returns with the value specified by the given key if it's exist, otherwise it's default value will be returned.
        /// </summary>
        /// <param name="key">The key of the entry.</param>
        /// <returns>The found or the default value.</returns>
        [MethodImpl(Constants.Inline)]
        public TValue GetOrDefault(TKey key)
        {
            var hash = key.GetHashCode();

            var node = this.repository;
            while (!node.IsEmpty && node.StoredHash != hash)
                node = hash < node.StoredHash ? node.LeftNode : node.RightNode;
            return !node.IsEmpty && (ReferenceEquals(key, node.StoredKey) || key.Equals(node.StoredKey)) ?
                node.StoredValue : node.Collisions.GetOrDefault(key);
        }

        /// <summary>
        /// Returns with the value specified by the given key if it's exist, otherwise it's default value will be returned.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="key">The key of the entry.</param>
        /// <returns>The found or the default value.</returns>
        [MethodImpl(Constants.Inline)]
        public TValue GetOrDefault(int hash, TKey key)
        {
            var node = this.repository;
            while (!node.IsEmpty && node.StoredHash != hash)
                node = hash < node.StoredHash ? node.LeftNode : node.RightNode;
            return !node.IsEmpty && (ReferenceEquals(key, node.StoredKey) || key.Equals(node.StoredKey)) ?
                node.StoredValue : node.Collisions.GetOrDefault(key);
        }

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
            var currentRepo = this.repository;
            var newRepo = this.repository.AddOrUpdate(hash, key, value, updateDelegate);

            Swap.SwapValue(ref this.repository, currentRepo, newRepo, repo => repo.AddOrUpdate(hash, key, value, updateDelegate));

            return this;
        }

        /// <inheritdoc />
        public IEnumerator<TValue> GetEnumerator() => this.repository.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.repository.GetEnumerator();
    }

    /// <summary>
    /// Represents a concurrent AVL tree
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class ConcurrentTree<TValue> : IEnumerable<TValue>
    {
        /// <summary>
        /// Static factory method.
        /// </summary>
        /// <returns>A new tree instance</returns>
        public static ConcurrentTree<TValue> Create() => new ConcurrentTree<TValue>();

        private AvlTree<TValue> repository;

        /// <summary>
        /// The current root value of the tree,
        /// </summary>
        public TValue Value => this.repository.Value;

        /// <summary>
        /// Inidicates that the tree has more nodes than the root one.
        /// </summary>
        public bool HasMultipleItems => this.repository.HasMultipleItems;

        /// <summary>
        /// True if the tree is empty, otherwise false.
        /// </summary>
        public bool IsEmpty => this.repository.IsEmpty;

        /// <summary>
        /// Constructs the <see cref="ConcurrentTree{TKey, TValue}"/>
        /// </summary>
        public ConcurrentTree()
        {
            this.repository = AvlTree<TValue>.Empty;
        }

        /// <summary>
        /// Returns with the value specified by the given key if it's exist, otherwise it's default value will be returned.
        /// </summary>
        /// <param name="key">The key of the entry.</param>
        /// <returns>The found or the default value.</returns>
        public TValue GetOrDefault(int key) =>
            this.repository.GetOrDefault(key);


        /// <summary>
        /// Inserts an item into the tree if it doesn't exist, otherwise the existing item will be replaced if the update delegate is set.
        /// </summary>
        /// <param name="key">The key of the entry.</param>
        /// <param name="value">The value which will be inserted if it doesn't exist yet.</param>
        /// <param name="updateDelegate">The update delegate which will be invoked when the value is already stored on the tree.</param>
        /// <returns>The modified tree.</returns>
        public ConcurrentTree<TValue> AddOrUpdate(int key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null)
        {
            var currentRepo = this.repository;
            var newRepo = this.repository.AddOrUpdate(key, value, updateDelegate);

            Swap.SwapValue(ref this.repository, currentRepo, newRepo, repo => repo.AddOrUpdate(key, value, updateDelegate));

            return this;
        }

        /// <inheritdoc />
        public IEnumerator<TValue> GetEnumerator() => this.repository.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.repository.GetEnumerator();
    }
}
