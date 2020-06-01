using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal sealed class ImmutableArray<TValue> : IEnumerable<TValue>
    {
        public static readonly ImmutableArray<TValue> Empty = new ImmutableArray<TValue>();

        private readonly TValue[] repository;

        public int Length;

        private ImmutableArray(TValue item, TValue[] old)
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

        public ImmutableArray()
        {
            this.repository = Constants.EmptyArray<TValue>();
        }

        public ImmutableArray<TValue> Add(TValue value) =>
            new ImmutableArray<TValue>(value, this.repository);

        public TValue this[int i] => this.repository[i];

        IEnumerator IEnumerable.GetEnumerator() => this.repository.GetEnumerator();

        public IEnumerator<TValue> GetEnumerator()
        {
            for (var i = 0; i < this.Length; i++)
                yield return this.repository[i];
        }
    }

    internal sealed class ImmutableArray<TKey, TValue> : IEnumerable<TValue>
    {
        public static readonly ImmutableArray<TKey, TValue> Empty = new ImmutableArray<TKey, TValue>();

        public KeyValue<TKey, TValue>[] Repository;

        public int Length;

        private ImmutableArray(KeyValue<TKey, TValue> item, KeyValue<TKey, TValue>[] old)
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

        public ImmutableArray(KeyValue<TKey, TValue>[] initial)
        {
            this.Repository = initial;
            this.Length = initial.Length;
        }

        public ImmutableArray(TKey key, TValue value)
        {
            this.Repository = new[] { new KeyValue<TKey, TValue>(key, value) };
            this.Length = 1;
        }

        public ImmutableArray()
        {
            this.Repository = Constants.EmptyArray<KeyValue<TKey, TValue>>();
        }

        public ImmutableArray<TKey, TValue> Add(TKey key, TValue value) =>
           new ImmutableArray<TKey, TValue>(new KeyValue<TKey, TValue>(key, value), this.Repository);

        public ImmutableArray<TKey, TValue> AddOrUpdate(TKey key, TValue value, bool byRef, bool allowUpdate = true, Action<TValue, TValue> updateAction = null)
        {
            var length = this.Length;
            var count = length - 1;
            while (count >= 0 && (byRef && !ReferenceEquals(this.Repository[count].Key, key) || !byRef && !Equals(this.Repository[count].Key, key))) count--;

            if (count == -1)
                return this.Add(key, value);

            if (!allowUpdate)
                return this;

            var newRepository = new KeyValue<TKey, TValue>[length];
            Array.Copy(this.Repository, newRepository, length);

            updateAction?.Invoke(newRepository[count].Value, value);

            newRepository[count] = new KeyValue<TKey, TValue>(key, value);
            return new ImmutableArray<TKey, TValue>(newRepository);
        }

        [MethodImpl(Constants.Inline)]
        public TValue GetOrDefault(TKey key, bool byRef)
        {
            var length = this.Length;
            for (var i = 0; i < length; i++)
            {
                var item = this.Repository[i];
                if (byRef && ReferenceEquals(item.Key, key) || !byRef && Equals(item.Key, key))
                    return item.Value;
            }

            return default;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.Repository.GetEnumerator();

        public IEnumerator<TValue> GetEnumerator()
        {
            for (var i = 0; i < this.Length; i++)
                yield return this.Repository[i].Value;
        }
    }
}
