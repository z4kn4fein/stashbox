using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal class HashMap<TKey, TValue>
    {
        protected readonly int IndexBound;
        private readonly AvlTreeKeyValue<TKey, TValue>[] array;

        public HashMap(int arraySize = 64)
        {
            this.IndexBound = arraySize - 1;
            this.array = new AvlTreeKeyValue<TKey, TValue>[arraySize];

            for (var i = 0; i < arraySize; i++)
                this.array[i] = AvlTreeKeyValue<TKey, TValue>.Empty;
        }

        [MethodImpl((MethodImplOptions)256)]
        public TValue GetOrDefault(TKey key)
        {
            var hash = key.GetHashCode();

            return this.array[hash & this.IndexBound].GetOrDefault(hash, key);
        }

        [MethodImpl((MethodImplOptions)256)]
        public HashMap<TKey, TValue> AddOrUpdate(TKey key, TValue value)
        {
            var hash = key.GetHashCode();

            var index = hash & this.IndexBound;
            this.array[index] = this.array[index].AddOrUpdate(hash, key, value);
            return this;
        }
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
