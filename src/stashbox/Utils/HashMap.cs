using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal class HashMap<TKey, TValue> : IEnumerable<TValue>
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

        [MethodImpl(Constants.Inline)]
        public TValue GetOrDefault(TKey key)
        {
            var hash = key.GetHashCode();

            var tree = this.array[hash & this.IndexBound];
            if (tree == null)
                return default(TValue);

            while (!tree.IsEmpty && tree.StoredHash != hash)
                tree = hash < tree.StoredHash ? tree.LeftNode : tree.RightNode;
            return !tree.IsEmpty && (ReferenceEquals(key, tree.StoredKey) || key.Equals(tree.StoredKey)) ?
                tree.Value : tree.Collisions.GetOrDefault(key);
        }

        [MethodImpl(Constants.Inline)]
        public void AddOrUpdate(TKey key, TValue value, Func<TValue, TValue, TValue> updateDelegate = null)
        {
            var hash = key.GetHashCode();

            var treeIndex = hash & this.IndexBound;

            var tree = this.array[treeIndex] ?? AvlTreeKeyValue<TKey, TValue>.Empty;
            Swap.SwapValue(ref this.array[treeIndex], tree, tree.AddOrUpdate(hash, key, value, updateDelegate),
                t => t.AddOrUpdate(hash, key, value, updateDelegate));
        }

        public void Clear() => this.array = new AvlTreeKeyValue<TKey, TValue>[this.arraySize];

        public IEnumerator<TValue> GetEnumerator()
        {
            for (var i = 0; i < arraySize; i++)
                if (this.array[i] != null)
                    foreach (var item in this.array)
                        yield return item.StoredValue;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
