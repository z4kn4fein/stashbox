namespace Stashbox.Utils
{
    internal class HashMap<TKey, TValue>
    {
        private readonly int arraySize;
        protected readonly int IndexBound;
        private AvlTreeKeyValue<TKey, TValue>[] array;

        public HashMap(int arraySize = 64)
        {
            this.arraySize = arraySize;
            this.IndexBound = arraySize - 1;
            this.array = new AvlTreeKeyValue<TKey, TValue>[arraySize];
        }

        public TValue GetOrDefault(TKey key)
        {
            var hash = key.GetHashCode();

            var tree = this.array[hash & this.IndexBound];
            return tree == null ? default(TValue) : tree.GetOrDefault(hash, key);
        }

        public HashMap<TKey, TValue> AddOrUpdate(TKey key, TValue value)
        {
            var hash = key.GetHashCode();

            var index = hash & this.IndexBound;

            var tree = this.array[index] ?? AvlTreeKeyValue<TKey, TValue>.Empty;
            var newTree = tree.AddOrUpdate(hash, key, value);
            this.array[index] = newTree;
            return this;
        }

        public void Clear() => this.array = new AvlTreeKeyValue<TKey, TValue>[this.arraySize];
    }

    internal class HashMap<TKey, TNestedKey, TValue>
    {
        private readonly int arraySize;
        protected readonly int IndexBound;
        private AvlTreeKeyValue<TNestedKey, TValue>[] array;

        public HashMap(int arraySize = 64)
        {
            this.arraySize = arraySize;
            this.IndexBound = arraySize - 1;
            this.array = new AvlTreeKeyValue<TNestedKey, TValue>[arraySize];
        }

        public TValue GetOrDefault(TKey key, TNestedKey nestedKey)
        {
            var hash = key.GetHashCode();
            var nestedHash = nestedKey.GetHashCode();

            var tree = this.array[hash & this.IndexBound];
            return tree == null ? default(TValue) : tree.GetOrDefault(nestedHash, nestedKey);
        }

        public HashMap<TKey, TNestedKey, TValue> AddOrUpdate(TKey key, TNestedKey nestedKey, TValue value)
        {
            var hash = key.GetHashCode();
            var nestedHash = nestedKey.GetHashCode();

            var index = hash & this.IndexBound;

            var tree = this.array[index] ?? AvlTreeKeyValue<TNestedKey, TValue>.Empty;
            var newTree = tree.AddOrUpdate(nestedHash, nestedKey, value);
            this.array[index] = newTree;
            return this;
        }

        public void Clear() => this.array = new AvlTreeKeyValue<TNestedKey, TValue>[this.arraySize];
    }
}
