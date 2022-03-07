using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils.Data.Immutable
{
    internal class ImmutableBucket<TValue>
    {
        public static readonly ImmutableBucket<TValue> Empty = new(Constants.EmptyArray<TValue>());

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

        internal ImmutableBucket<TValue> Insert(int index, TValue value)
        {
            if (index > this.Length - 1)
                throw new IndexOutOfRangeException();

            if (this.Length == 0)
                return new ImmutableBucket<TValue>(new[] { value });

            var newRepository = new TValue[this.Length + 1];
            Array.Copy(this.Repository, newRepository, index);
            newRepository[index] = value;
            Array.Copy(this.Repository, index, newRepository, index + 1, this.Length - index);

            return new ImmutableBucket<TValue>(newRepository);
        }
    }

    internal class ImmutableBucket<TKey, TValue> : IEnumerable<TValue>
    {
        public static readonly ImmutableBucket<TKey, TValue> Empty = new(Constants.EmptyArray<ReadOnlyKeyValue<TKey, TValue>>());

        public readonly int Length;

        public readonly ReadOnlyKeyValue<TKey, TValue>[] Repository;

        public ImmutableBucket(TKey key, TValue value)
            : this(new[] { new ReadOnlyKeyValue<TKey, TValue>(key, value) })
        { }

        public ImmutableBucket(ReadOnlyKeyValue<TKey, TValue>[] repository)
        {
            this.Repository = repository;
            this.Length = repository.Length;
        }

        internal ImmutableBucket<TKey, TValue> Add(TKey key, TValue value)
        {
            if (this.Length == 0)
                return new ImmutableBucket<TKey, TValue>(new[] { new ReadOnlyKeyValue<TKey, TValue>(key, value) });

            var newRepository = new ReadOnlyKeyValue<TKey, TValue>[this.Length + 1];
            Array.Copy(this.Repository, newRepository, this.Length);
            newRepository[this.Length] = new ReadOnlyKeyValue<TKey, TValue>(key, value);

            return new ImmutableBucket<TKey, TValue>(newRepository);
        }

        internal ImmutableBucket<TKey, TValue> AddOrUpdate(TKey key, TValue value, bool byRef, Func<TValue, TValue, TValue>? update = null)
        {
            if (this.Length == 0)
                return new ImmutableBucket<TKey, TValue>(new[] { new ReadOnlyKeyValue<TKey, TValue>(key, value) });

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

            var old = this.Repository[count].Value;
            var @new = update == null ? value : update(old, value);
            if (ReferenceEquals(@new, old))
                return this;

            var newRepository = new ReadOnlyKeyValue<TKey, TValue>[this.Length];
            Array.Copy(this.Repository, newRepository, this.Length);
            newRepository[count] = new ReadOnlyKeyValue<TKey, TValue>(key, @new);

            return new ImmutableBucket<TKey, TValue>(newRepository);
        }

        internal ImmutableBucket<TKey, TValue> ReplaceIfExists(TKey key, Func<TValue, TValue> updateDelegate, bool byRef)
        {
            if (this.Length == 0)
                return this;

            var count = this.Length - 1;
            while (count >= 0)
            {
                ref readonly var item = ref this.Repository[count];
                if (byRef && ReferenceEquals(item.Key, key) || !byRef && Equals(item.Key, key))
                    break;

                count--;
            }

            if (count == -1)
                return this;

            var length = this.Length - 1;
            var newRepository = new ReadOnlyKeyValue<TKey, TValue>[length];
            Array.Copy(this.Repository, newRepository, length);

            newRepository[count] = new ReadOnlyKeyValue<TKey, TValue>(key, updateDelegate(newRepository[count].Value));

            return new ImmutableBucket<TKey, TValue>(newRepository);
        }

        internal ImmutableBucket<TKey, TValue> ReplaceIfExists(TKey key, TValue value, bool byRef, Func<TValue, TValue, TValue>? update = null)
        {
            if (this.Length == 0)
                return this;

            var count = this.Length - 1;
            while (count >= 0)
            {
                ref readonly var item = ref this.Repository[count];
                if (byRef && ReferenceEquals(item.Key, key) || !byRef && Equals(item.Key, key))
                    break;

                count--;
            }

            if (count == -1)
                return this;

            var old = this.Repository[count].Value;
            var @new = update == null ? value : update(old, value);
            if (ReferenceEquals(@new, old))
                return this;

            var newRepository = new ReadOnlyKeyValue<TKey, TValue>[this.Length];
            Array.Copy(this.Repository, newRepository, this.Length);

            newRepository[count] = new ReadOnlyKeyValue<TKey, TValue>(key, @new);

            return new ImmutableBucket<TKey, TValue>(newRepository);
        }

        [MethodImpl(Constants.Inline)]
        public TValue? GetOrDefaultByValue(TKey key)
        {
            var length = this.Repository.Length;
            for (var i = 0; i < length; i++)
            {
                ref readonly var item = ref this.Repository[i];
                if (Equals(item.Key, key))
                    return item.Value;
            }

            return default;
        }

        [MethodImpl(Constants.Inline)]
        public TValue? GetOrDefaultByRef(TKey key)
        {
            var length = this.Repository.Length;
            for (var i = 0; i < length; i++)
            {
                ref readonly var item = ref this.Repository[i];
                if (ReferenceEquals(item.Key, key))
                    return item.Value;
            }

            return default;
        }

        public ReadOnlyKeyValue<TKey, TValue> First() => this.Repository[0];

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
