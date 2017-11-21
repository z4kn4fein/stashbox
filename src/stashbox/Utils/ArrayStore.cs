using Stashbox.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal class ArrayStore<TValue> : IEnumerable<TValue>
    {
        public static readonly ArrayStore<TValue> Empty = new ArrayStore<TValue>();

        private readonly TValue[] repository;

        public int Length;

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

        private ArrayStore(TValue[] items, TValue[] old)
        {
            if (old.Length == 0)
            {
                this.repository = items;
                this.Length = items.Length;
            }
            else
            {
                var oldLength = old.Length;
                var itemsLength = items.Length;
                var newLength = oldLength + itemsLength;
                this.repository = new TValue[newLength];
                Array.Copy(old, this.repository, oldLength);
                Array.Copy(items, 0, this.repository, oldLength, itemsLength);
                this.Length = newLength;
            }
        }

        public ArrayStore()
        {
            this.repository = new TValue[0];
        }

        public ArrayStore(TValue[] initial)
        {
            this.repository = initial;
            this.Length = initial.Length;
        }

        public ArrayStore<TValue> Add(TValue value) =>
            new ArrayStore<TValue>(value, this.repository);

        public ArrayStore<TValue> AddRange(TValue[] items) =>
            new ArrayStore<TValue>(items, this.repository);

        public TValue this[int i] => this.repository[i];

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
        public static readonly ArrayStoreKeyed<TKey, TValue> Empty = new ArrayStoreKeyed<TKey, TValue>();

        public KeyValue<TKey, TValue>[] Repository { get; }

        public TValue Last => this.Repository[this.Length - 1].Value;

        public int Length { get; }

        private ArrayStoreKeyed(KeyValue<TKey, TValue> item, KeyValue<TKey, TValue>[] old)
        {
            if (old.Length == 0)
                this.Repository = new[] { item };
            else
            {
                this.Repository = new KeyValue<TKey, TValue>[old.Length + 1];
                Array.Copy(old, this.Repository, old.Length);
                this.Repository[old.Length] = item;
            }

            this.Length = old.Length + 1;
        }

        internal ArrayStoreKeyed(KeyValue<TKey, TValue>[] initial)
        {
            this.Repository = initial;
            this.Length = initial.Length;
        }

        public ArrayStoreKeyed()
        {
            this.Repository = new KeyValue<TKey, TValue>[0];
        }

        public TValue this[int i] => this.Repository[i].Value;

        public ArrayStoreKeyed<TKey, TValue> Add(TKey key, TValue value) =>
           new ArrayStoreKeyed<TKey, TValue>(new KeyValue<TKey, TValue>(key, value), this.Repository);

        public ArrayStoreKeyed<TKey, TValue> AddOrUpdate(TKey key, TValue value, bool allowUpdate = true)
        {
            var length = this.Repository.Length;
            var count = length - 1;
            while (count >= 0 && !Equals(this.Repository[count].Key, key)) count--;

            if (count == -1)
                return this.Add(key, value);

            if (!allowUpdate)
                return this;

            var newRepository = new KeyValue<TKey, TValue>[length];
            Array.Copy(this.Repository, newRepository, length);
            newRepository[count] = new KeyValue<TKey, TValue>(key, value);
            return new ArrayStoreKeyed<TKey, TValue>(newRepository);
        }

        [MethodImpl(Constants.Inline)]
        public TValue GetOrDefault(TKey key)
        {
            var length = this.Repository.Length;
            for (var i = 0; i < length; i++)
            {
                var item = this.Repository[i];
                if (item.Key.Equals(key))
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
