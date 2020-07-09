using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils.Data.Immutable
{
    internal class ImmutableBucket<TValue>
    {
        public static readonly ImmutableBucket<TValue> Empty = new ImmutableBucket<TValue>(Constants.EmptyArray<TValue>());

        public readonly int Length;

        public readonly TValue[] Repository;

        public TValue this[int i] => this.Repository[i];

        public ImmutableBucket(TValue[] repository)
        {
            this.Repository = repository;
            this.Length = repository.Length;
        }

        internal ImmutableBucket<TValue> Add(TValue value)
        {
            if (this.Length == 0)
                return new ImmutableBucket<TValue>(new[] { value });

            var newRepository = new TValue[this.Length + 1];
            Array.Copy(this.Repository, newRepository, this.Length);
            newRepository[this.Length] = value;

            return new ImmutableBucket<TValue>(newRepository);
        }
    }

    internal class ImmutableBucket<TKey, TValue> : IEnumerable<TValue>
    {
        public static readonly ImmutableBucket<TKey, TValue> Empty = new ImmutableBucket<TKey, TValue>(Constants.EmptyArray<KeyValue<TKey, TValue>>());

        public readonly int Length;

        public readonly KeyValue<TKey, TValue>[] Repository;

        public ImmutableBucket(KeyValue<TKey, TValue>[] repository)
        {
            this.Repository = repository;
            this.Length = repository.Length;
        }

        internal ImmutableBucket<TKey, TValue> Add(TKey key, TValue value)
        {
            if (this.Length == 0)
                return new ImmutableBucket<TKey, TValue>(new[] { new KeyValue<TKey, TValue>(key, value) });

            var newRepository = new KeyValue<TKey, TValue>[this.Length + 1];
            Array.Copy(this.Repository, newRepository, this.Length);
            newRepository[this.Length] = new KeyValue<TKey, TValue>(key, value);

            return new ImmutableBucket<TKey, TValue>(newRepository);
        }

        internal ImmutableBucket<TKey, TValue> AddOrUpdate(TKey key, TValue value, bool byRef, Func<TValue, TValue, TValue> update = null)
        {
            if (this.Length == 0)
                return new ImmutableBucket<TKey, TValue>(new[] { new KeyValue<TKey, TValue>(key, value) });

            var count = this.Length - 1;
            while (count >= 0)
            {
                ref readonly var item = ref this.Repository[count];
                if (byRef && ReferenceEquals(item.Key, key) || !byRef && Equals(item.Key, key))
                    break;

                count--;
            }

            if (count == -1)
                return this.Add(key, value);

            var newRepository = new KeyValue<TKey, TValue>[this.Length];
            Array.Copy(this.Repository, newRepository, this.Length);

            value = update == null ? value : update.Invoke(newRepository[count].Value, value);
            newRepository[count] = new KeyValue<TKey, TValue>(key, value);

            return new ImmutableBucket<TKey, TValue>(newRepository);
        }

        [MethodImpl(Constants.Inline)]
        public TValue GetOrDefault(TKey key, bool byRef)
        {
            var length = this.Repository.Length;
            for (var i = 0; i < length; i++)
            {
                ref readonly var item = ref this.Repository[i];
                if (byRef && ReferenceEquals(item.Key, key) || !byRef && Equals(item.Key, key))
                    return item.Value;
            }

            return default;
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            for (var i = 0; i < this.Length; i++)
                yield return this.Repository[i].Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Repository.GetEnumerator();
        }
    }
}
