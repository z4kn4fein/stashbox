using System;

namespace Stashbox.Utils
{
    internal class HashMap<TKey, TValue>
    {
        private readonly RandomAccessArray<AvlTree<TValue>> repository;

        public HashMap()
        {
            this.repository = new RandomAccessArray<AvlTree<TValue>>();
        }

        public TValue GetOrDefault(TKey key)
        {
            var hash = key.GetHashCode();

            var tree = this.repository.Load(hash);
            return tree == null ? default(TValue) : tree.GetOrDefault(hash);
        }

        public void AddOrUpdate(TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null)
        {
            var hash = key.GetHashCode();

            var tree = this.repository.Load(hash) ?? new AvlTree<TValue>();
            var newTree = tree.AddOrUpdate(hash, value, updateDelegate);
            this.repository.Store(hash, newTree);
        }
    }
}
