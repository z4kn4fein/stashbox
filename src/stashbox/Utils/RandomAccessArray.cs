using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stashbox.Utils
{
    internal class RandomAccessArray<TKey, TValue>
    {
        private readonly int arraySize;
        private readonly TValue[] array;

        public RandomAccessArray(int arraySize = 64)
        {
            this.arraySize = arraySize;
            this.array = new TValue[arraySize];
        }

        public void Store(TKey key, TValue value) => this.array[GetIndex(key)] = value;

        public TValue Load(TKey key) => this.array[GetIndex(key)];

        private int GetIndex(TKey key) => key.GetHashCode() & (this.arraySize - 1);
    }
}
