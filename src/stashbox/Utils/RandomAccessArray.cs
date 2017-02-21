namespace Stashbox.Utils
{
    internal class RandomAccessArray<TValue>
    {
        private readonly int arraySize;
        protected readonly int IndexBound;
        private TValue[] array;

        public RandomAccessArray(int arraySize = 64)
        {
            this.arraySize = arraySize;
            this.IndexBound = arraySize - 1;
            this.array = new TValue[arraySize];
        }

        public void Store(int key, TValue value) => this.array[key & this.IndexBound] = value;

        public TValue Load(int key) => this.array[key & this.IndexBound];

        public void Clear() => this.array = new TValue[this.arraySize];
    }
}
