using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal class HashMap<TKey, TValue>
    {
        private readonly int arraySize;
        protected readonly int IndexBound;
        private readonly AvlTreeKeyValue<TKey, TValue>[] array;

        public HashMap(int arraySize = 64)
        {
            this.arraySize = arraySize;
            this.IndexBound = arraySize - 1;
            this.array = new AvlTreeKeyValue<TKey, TValue>[arraySize];

            for (var i = 0; i < arraySize; i++)
                this.array[i] = AvlTreeKeyValue<TKey, TValue>.Empty;
        }

        [MethodImpl(Constants.Inline)]
        public TValue GetOrDefault(TKey key)
        {
            var hash = key.GetHashCode();

            return this.array[hash & this.IndexBound].GetOrDefault(hash, key);
        }

        [MethodImpl(Constants.Inline)]
        public void AddOrUpdate(TKey key, TValue value)
        {
            var hash = key.GetHashCode();

            var index = hash & this.IndexBound;

            var tree = this.array[index];
            var newTree = tree.AddOrUpdate(hash, key, value);

            Swap.SwapValue(ref this.array[index], tree, newTree, root => root.AddOrUpdate(hash, key, value));
        }

        public void Clear() => this.Init();

        private void Init()
        {
            for (var i = 0; i < this.arraySize; i++)
                this.array[i] = AvlTreeKeyValue<TKey, TValue>.Empty;
        }
    }
}
