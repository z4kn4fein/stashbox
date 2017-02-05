using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stashbox.Utils
{
    internal class RandomAccessArray<TKey, TValue>
    {
        protected readonly int ArraySize;
        private readonly TValue[] array;

        public RandomAccessArray(int arraySize = 64)
        {
            this.ArraySize = arraySize;
            this.array = new TValue[arraySize];
        }

        public void Store(TKey key, TValue value) => this.array[GetIndex(key)] = value;

        public TValue Load(TKey key) => this.array[GetIndex(key)];

        protected virtual int GetIndex(TKey key) => key.GetHashCode() & (this.ArraySize - 1);
    }

    internal class IntRandomAccessArray<TValue> : RandomAccessArray<int, TValue>
    {
        protected override int GetIndex(int key) => key & (this.ArraySize - 1);
    }
}
