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

        public HashMap<TKey, TValue> AddOrUpdate(TKey key, TValue value)
        {
            var hash = key.GetHashCode();

            var tree = this.repository.Load(hash) ?? new AvlTree<TValue>();
            var newTree = tree.AddOrUpdate(hash, value);
            this.repository.Store(hash, newTree);
            return this;
        }

        public void Clear() => this.repository.Clear();
    }

    internal class HashMap<TKey, TNestedKey, TValue>
    {
        private readonly RandomAccessArray<AvlTree<TValue>> repository;

        public HashMap()
        {
            this.repository = new RandomAccessArray<AvlTree<TValue>>();
        }

        public TValue GetOrDefault(TKey key, TNestedKey nestedKey)
        {
            var hash = key.GetHashCode();
            var nestedHash = nestedKey.GetHashCode();

            var tree = this.repository.Load(hash);
            return tree == null ? default(TValue) : tree.GetOrDefault(nestedHash);
        }

        public HashMap<TKey, TNestedKey, TValue> AddOrUpdate(TKey key, TNestedKey nestedKey, TValue value)
        {
            var hash = key.GetHashCode();
            var nestedHash = nestedKey.GetHashCode();

            var tree = this.repository.Load(hash) ?? new AvlTree<TValue>();
            var newTree = tree.AddOrUpdate(nestedHash, value);
            this.repository.Store(hash, newTree);
            return this;
        }

        public void Clear() => this.repository.Clear();
    }
}
