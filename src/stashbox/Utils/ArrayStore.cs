using System;
using System.Collections.Generic;

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

    internal class ArrayStoreKeyed<TKey, TValue>
    {
        public static ArrayStoreKeyed<TKey, TValue> Empty = new ArrayStoreKeyed<TKey, TValue>();

        private readonly KeyValuePair<TKey, TValue>[] repository;

        public int Length { get; }

        private ArrayStoreKeyed(KeyValuePair<TKey, TValue> item, KeyValuePair<TKey, TValue>[] old)
        {
            if (old.Length == 0)
                this.repository = new[] { item };
            else
            {
                this.repository = new KeyValuePair<TKey, TValue>[old.Length + 1];
                Array.Copy(old, this.repository, old.Length);
                this.repository[old.Length] = item;
            }

            this.Length = old.Length + 1;
        }

        private ArrayStoreKeyed(KeyValuePair<TKey, TValue>[] initial)
        {
            this.repository = initial;
            this.Length = initial.Length;
        }

        public ArrayStoreKeyed()
        {
            this.repository = new KeyValuePair<TKey, TValue>[0];
        }

        public ArrayStoreKeyed<TKey, TValue> AddOrUpdate(TKey key, TValue value, out bool updated)
        {
            var length = this.repository.Length;
            var count = length - 1;
            while (count >= 0 && !this.repository[count].Key.Equals(key)) count--;

            if (count == -1)
            {
                updated = false;
                return new ArrayStoreKeyed<TKey, TValue>(new KeyValuePair<TKey, TValue>(key, value), this.repository);
            }


            var newRepository = new KeyValuePair<TKey, TValue>[length];
            Array.Copy(this.repository, newRepository, length);
            updated = true;
            return new ArrayStoreKeyed<TKey, TValue>(newRepository);
        }

        public TValue GetOrDefault(TKey key)
        {
            var length = this.repository.Length;
            for (var i = 0; i < length; i++)
            {
                var item = this.repository[i];
                if (item.Key.Equals(key))
                    return item.Value;
            }

            return default(TValue);
        }
    }
}
