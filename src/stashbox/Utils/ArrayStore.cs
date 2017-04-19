using System;
using System.Collections;
using System.Collections.Generic;

namespace Stashbox.Utils
{
    internal class ArrayStore<TValue> : IEnumerable<TValue>
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

        IEnumerator IEnumerable.GetEnumerator() => this.repository.GetEnumerator();

        public IEnumerator<TValue> GetEnumerator()
        {
            for (var i = 0; i < this.Length; i++)
                yield return this.repository[i];
        }
    }

    internal class ArrayStoreKeyed<TKey, TValue> : IEnumerable<TValue>
    {
        public static ArrayStoreKeyed<TKey, TValue> Empty = new ArrayStoreKeyed<TKey, TValue>();

        public KeyValuePair<TKey, TValue>[] Repository { get; }

        public TValue Last => this.Repository[this.Length - 1].Value;

        public int Length { get; }

        private ArrayStoreKeyed(KeyValuePair<TKey, TValue> item, KeyValuePair<TKey, TValue>[] old)
        {
            if (old.Length == 0)
                this.Repository = new[] { item };
            else
            {
                this.Repository = new KeyValuePair<TKey, TValue>[old.Length + 1];
                Array.Copy(old, this.Repository, old.Length);
                this.Repository[old.Length] = item;
            }

            this.Length = old.Length + 1;
        }

        private ArrayStoreKeyed(KeyValuePair<TKey, TValue>[] initial)
        {
            this.Repository = initial;
            this.Length = initial.Length;
        }

        public ArrayStoreKeyed()
        {
            this.Repository = new KeyValuePair<TKey, TValue>[0];
        }

        public ArrayStoreKeyed<TKey, TValue> Add(TKey key, TValue value) =>
           new ArrayStoreKeyed<TKey, TValue>(new KeyValuePair<TKey, TValue>(key, value), this.Repository);

        public ArrayStoreKeyed<TKey, TValue> AddOrUpdate(TKey key, TValue value, out bool updated, bool allowUpdate = true)
        {
            var length = this.Repository.Length;
            var count = length - 1;
            while (count >= 0 && !this.Repository[count].Key.Equals(key)) count--;

            if (count == -1)
            {
                updated = false;
                return this.Add(key, value);
            }

            if (!allowUpdate)
            {
                updated = false;
                return this;
            }

            var newRepository = new KeyValuePair<TKey, TValue>[length];
            Array.Copy(this.Repository, newRepository, length);
            newRepository[count] = new KeyValuePair<TKey, TValue>(key, value);
            updated = true;
            return new ArrayStoreKeyed<TKey, TValue>(newRepository);
        }

        public ArrayStoreKeyed<TKey, TValue> AddOrUpdate(TKey key, TValue value, bool allowUpdate = true) =>
            this.AddOrUpdate(key, value, out bool updated, allowUpdate);

        public TValue GetOrDefault(TKey key)
        {
            var length = this.Repository.Length;
            for (var i = 0; i < length; i++)
            {
                var item = this.Repository[i];
                if (ReferenceEquals(key, item.Key) || item.Key.Equals(key))
                    return item.Value;
            }

            return default(TValue);
        }

        IEnumerator IEnumerable.GetEnumerator() => this.Repository.GetEnumerator();

        public IEnumerator<TValue> GetEnumerator()
        {
            for (var i = 0; i < this.Length; i++)
                yield return this.Repository[i].Value;
        }
    }
}
