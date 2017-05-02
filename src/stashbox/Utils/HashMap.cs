using System.Runtime.CompilerServices;

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
        public void AddOrUpdate(TKey key, TValue value)
        {
            var hash = key.GetHashCode();

            var treeIndex = hash & this.IndexBound;

            var tree = this.array[treeIndex] ?? AvlTreeKeyValue<TKey, TValue>.Empty;
            this.array[treeIndex] = tree.AddOrUpdate(hash, key, value);
        }

        public void Clear() => this.array = new AvlTreeKeyValue<TKey, TValue>[this.arraySize];
    }
}
