using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils.Data.Immutable
{
    internal interface IImmutableArray<TKey, TValue> : IEnumerable<TValue>
    {
        IImmutableArray<TKey, TValue> AddOrUpdate(TKey key, TValue value, bool byRef, Func<TValue, TValue, TValue> update = null);

        IImmutableArray<TKey, TValue> Add(TKey key, TValue value);

        TValue GetOrDefault(TKey key, bool byRef);

        IEnumerable<KeyValue<TKey, TValue>> Walk();
    }

    internal class ImmutableBucket<TValue>
    {
        public static readonly ImmutableBucket<TValue> Empty = new ImmutableBucket<TValue>(Constants.EmptyArray<TValue>());

        public readonly int Length;

        private readonly TValue[] repository;

        public ImmutableBucket(TValue[] repository)
        {
            this.repository = repository;
            this.Length = repository.Length;
        }

        public TValue this[int i] => this.repository[i];

        internal ImmutableBucket<TValue> Add(TValue value)
        {
            if (this.Length == 0)
                return new ImmutableBucket<TValue>(new[] { value });

            var newRepository = new TValue[this.Length + 1];
            Array.Copy(this.repository, newRepository, this.Length);
            newRepository[this.Length] = value;

            return new ImmutableBucket<TValue>(newRepository);
        }
    }

    internal class ImmutableArray<TKey, TValue> : IImmutableArray<TKey, TValue>
    {
        public static readonly IImmutableArray<TKey, TValue> Empty = new ImmutableArray<TKey, TValue>(Constants.EmptyArray<ImmutableBucket>());

        private const int BucketLength = 32;

        public readonly int Length;

        private readonly ImmutableBucket[] repository;

        private ImmutableArray(ImmutableBucket[] repository)
        {
            this.repository = repository;
            this.Length = repository.Length == 0
                ? 0
                : (repository.Length - 1) * BucketLength +
                  repository[repository.Length - 1].Length;
        }

        public IImmutableArray<TKey, TValue> Add(TKey key, TValue value)
        {
            if (this.Length == 0)
                return new ImmutableBucket(new[] { new KeyValue<TKey, TValue>(key, value) });

            return this.AddInternal(key, value);
        }

        public IImmutableArray<TKey, TValue> AddOrUpdate(TKey key, TValue value, bool byRef, Func<TValue, TValue, TValue> update = null)
        {
            if (this.Length == 0)
                return new ImmutableBucket(new[] { new KeyValue<TKey, TValue>(key, value) });

            var length = this.repository.Length;
            var count = length - 1;

            ImmutableBucket updated = default;
            while (count >= 0 && !this.repository[count].TryUpdate(key, value, byRef, out updated, update)) count--;

            if (count == -1)
                return this.AddInternal(key, value);

            var newRepository = new ImmutableBucket[length];
            Array.Copy(this.repository, newRepository, length);

            newRepository[count] = updated;
            return new ImmutableArray<TKey, TValue>(newRepository);
        }

        private IImmutableArray<TKey, TValue> AddInternal(TKey key, TValue value)
        {
            var repositoryLength = this.repository.Length;
            var lastBucketIndex = repositoryLength - 1;
            var lastBucket = this.repository[lastBucketIndex];
            if (lastBucket.Length >= BucketLength)
            {
                var newRepository = new ImmutableBucket[repositoryLength + 1];
                Array.Copy(this.repository, newRepository, repositoryLength);
                newRepository[repositoryLength] = new ImmutableBucket(new[] { new KeyValue<TKey, TValue>(key, value) });

                return new ImmutableArray<TKey, TValue>(newRepository);
            }

            var updatedRepository = new ImmutableBucket[repositoryLength];
            Array.Copy(this.repository, updatedRepository, repositoryLength);

            updatedRepository[lastBucketIndex] = lastBucket.AddInternal(key, value);
            return new ImmutableArray<TKey, TValue>(updatedRepository);
        }

        [MethodImpl(Constants.Inline)]
        public TValue GetOrDefault(TKey key, bool byRef)
        {
            var length = this.repository.Length;
            for (var i = 0; i < length; i++)
            {
                var bucketLength = this.repository[i].Length;
                for (var j = 0; j < bucketLength; j++)
                {
                    var repo = this.repository[i];
                    ref readonly var item = ref repo[j];
                    if (byRef && ReferenceEquals(item.Key, key) || !byRef && Equals(item.Key, key))
                        return item.Value;
                }
            }

            return default;
        }

        public IEnumerable<KeyValue<TKey, TValue>> Walk()
        {
            var length = this.repository.Length;
            for (var i = 0; i < length; i++)
            {
                var bucketLength = this.repository[i].Length;
                for (var j = 0; j < bucketLength; j++)
                    yield return this.repository[i][j];
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            var length = this.repository.Length;
            for (var i = 0; i < length; i++)
            {
                var bucketLength = this.repository[i].Length;
                for (var j = 0; j < bucketLength; j++)
                    yield return this.repository[i][j].Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class ImmutableBucket : IImmutableArray<TKey, TValue>
        {
            public readonly int Length;

            private readonly KeyValue<TKey, TValue>[] repository;

            public ImmutableBucket(KeyValue<TKey, TValue>[] repository)
            {
                this.repository = repository;
                this.Length = repository.Length;
            }

            public ref KeyValue<TKey, TValue> this[int i] => ref this.repository[i];

            public IImmutableArray<TKey, TValue> Add(TKey key, TValue value) =>
                this.Length >= BucketLength
                    ? new ImmutableArray<TKey, TValue>(new[] { this }).AddInternal(key, value)
                    : this.AddInternal(key, value);

            public IImmutableArray<TKey, TValue> AddOrUpdate(TKey key, TValue value, bool byRef, Func<TValue, TValue, TValue> update = null)
            {
                if (this.Length >= BucketLength)
                    return new ImmutableArray<TKey, TValue>(new[] { this }).AddOrUpdate(key, value, byRef, update);

                return this.TryUpdate(key, value, byRef, out var updated, update) ? updated : this.AddInternal(key, value);
            }

            [MethodImpl(Constants.Inline)]
            public TValue GetOrDefault(TKey key, bool byRef)
            {
                var length = this.repository.Length;
                for (var i = 0; i < length; i++)
                {
                    ref readonly var item = ref this.repository[i];
                    if (byRef && ReferenceEquals(item.Key, key) || !byRef && Equals(item.Key, key))
                        return item.Value;
                }

                return default;
            }

            internal ImmutableBucket AddInternal(TKey key, TValue value)
            {
                var newRepository = new KeyValue<TKey, TValue>[this.Length + 1];
                Array.Copy(this.repository, newRepository, this.Length);
                newRepository[this.Length] = new KeyValue<TKey, TValue>(key, value);

                return new ImmutableBucket(newRepository);
            }

            internal bool TryUpdate(TKey key, TValue value, bool byRef, out ImmutableBucket updated, Func<TValue, TValue, TValue> update = null)
            {
                updated = default;
                var length = this.Length;
                var count = length - 1;
                while (count >= 0)
                {
                    ref readonly var item = ref this.repository[count];
                    if (byRef && ReferenceEquals(item.Key, key) || !byRef && Equals(item.Key, key))
                        break;

                    count--;
                }

                if (count == -1)
                    return false;

                var newRepository = new KeyValue<TKey, TValue>[length];
                Array.Copy(this.repository, newRepository, length);

                value = update == null ? value : update.Invoke(newRepository[count].Value, value);

                newRepository[count] = new KeyValue<TKey, TValue>(key, value);
                updated = new ImmutableBucket(newRepository);
                return true;
            }

            public IEnumerable<KeyValue<TKey, TValue>> Walk()
            {
                for (var i = 0; i < this.Length; i++)
                    yield return this.repository[i];
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                for (var i = 0; i < this.Length; i++)
                    yield return this.repository[i].Value;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
