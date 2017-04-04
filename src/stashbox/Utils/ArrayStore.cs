using System;

namespace Stashbox.Utils
{
    internal class ArrayStore<TValue>
    {
        public static ArrayStore<TValue> Empty = new ArrayStore<TValue>();

        private readonly TValue[] repository;

        public int Length { get; }

        private ArrayStore(TValue item, TValue[] old)
        {
            if (old.Length == 0)
                this.repository = new[] { item };
            else
            {
                this.repository = new TValue[old.Length + 1];
                Array.Copy(old, this.repository, old.Length);
                this.repository[old.Length] = item;
            }

            this.Length = old.Length + 1;
        }

        public ArrayStore()
        {
            this.repository = new TValue[0];
        }

        public ArrayStore<TValue> Add(TValue value) =>
            new ArrayStore<TValue>(value, this.repository);

        public TValue Get(int index) =>
            this.repository[index];
    }
}
