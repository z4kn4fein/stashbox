using System;

namespace Stashbox.Utils
{
    internal class SyncTree<TValue>
    {
        private AvlTree<TValue> repository;

        public SyncTree()
        {
            this.repository = new AvlTree<TValue>();
        }

        public TValue GetOrDefault(int key) => this.repository.GetOrDefault(key);

        public void AddOrUpdate(int key, TValue value, Func<TValue, TValue, TValue> update = null) =>
            this.repository = this.repository.AddOrUpdate(key, value, update);
    }

    internal class SyncTree<TKey, TValue>
    {
        private AvlTree<TValue> repository;

        public SyncTree()
        {
            this.repository = new AvlTree<TValue>();
        }

        public TValue GetOrDefault(TKey key)
        {
            var hash = key.GetHashCode();
            return this.repository.GetOrDefault(hash);
        }

        public void AddOrUpdate(TKey key, TValue value, Func<TValue, TValue, TValue> update = null)
        {
            var hash = key.GetHashCode();
            this.repository = this.repository.AddOrUpdate(hash, value, update);
        }
    }
}
