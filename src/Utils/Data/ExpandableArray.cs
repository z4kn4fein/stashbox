using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils.Data
{
    internal class ExpandableArray<TItem> : IEnumerable<TItem>
    {
        private const int InitialSize = 8;

        public static ExpandableArray<TItem> FromEnumerable(IEnumerable<TItem> initial) =>
            new ExpandableArray<TItem>(initial);

        public int Length;
        public bool IsEmpty => this.Length == 0;

        protected TItem[] Repository;

        public ExpandableArray()
        { }

        public ExpandableArray(IEnumerable<TItem> initial)
        {
            this.Repository = initial.ToArray();
            this.Length = this.Repository.Length;
        }

        public void Add(TItem item)
        {
            var index = this.EnsureSize();
            this.Repository[index] = item;
        }

        public void AddOrKeep(TItem item)
        {
            if (this.ContainsReference(item))
                return;

            var index = this.EnsureSize();
            this.Repository[index] = item;
        }

        public void AddRange(IEnumerable<TItem> items)
        {
            var asArray = items.CastToArray();
            var index = this.EnsureSize(asArray.Length);
            Array.Copy(asArray, 0, this.Repository, index, asArray.Length);
        }

        public ref TItem this[int i] => ref this.Repository[i];

        public TItem[] AsArray()
        {
            if (this.IsEmpty)
                return Constants.EmptyArray<TItem>();

            var newArray = new TItem[this.Length];
            Array.Copy(this.Repository, 0, newArray, 0, this.Length);
            return newArray;
        }

        public int IndexOf(TItem element)
        {
            var length = this.Length;
            if (length == 1) return ReferenceEquals(this.Repository[0], element) ? 0 : -1;

            for (var i = 0; i < length; i++)
                if (ReferenceEquals(this.Repository[i], element))
                    return i;

            return -1;
        }

        public bool ContainsReference(TItem element)
        {
            var length = this.Length;
            if (length == 1) return ReferenceEquals(this.Repository[0], element);

            for (var i = 0; i < length; i++)
                if (ReferenceEquals(this.Repository[i], element))
                    return true;

            return false;
        }

        public bool Contains(TItem element)
        {
            var length = this.Length;
            if (length == 1) return Equals(this.Repository[0], element);

            for (var i = 0; i < length; i++)
                if (Equals(this.Repository[i], element))
                    return true;

            return false;
        }

        protected int EnsureSize(int increaseAmount = 1)
        {
            if (this.Length == 0)
                this.Repository = new TItem[increaseAmount > InitialSize ? increaseAmount : InitialSize];

            this.Length += increaseAmount;
            if (this.Repository.Length >= this.Length) return this.Length - increaseAmount;

            var newSize = this.Repository.Length * 2;
            var desiredSize = this.Length > newSize ? this.Length : newSize;
            var newArray = new TItem[desiredSize];
            Array.Copy(this.Repository, 0, newArray, 0, this.Repository.Length);
            this.Repository = newArray;

            return this.Length - increaseAmount;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            var length = this.Length;
            for (var i = 0; i < length; i++)
                yield return this.Repository[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    internal class ExpandableArray<TKey, TItem> : ExpandableArray<KeyValue<TKey, TItem>>
    {
        [MethodImpl(Constants.Inline)]
        public TItem GetOrDefault(TKey key, bool byRef)
        {
            var length = this.Length;
            for (var i = 0; i < length; i++)
            {
                ref var item = ref this.Repository[i];
                if (byRef && ReferenceEquals(item.Key, key) || !byRef && Equals(item.Key, key))
                    return item.Value;
            }

            return default;
        }

        public void AddOrKeep(TKey item, TItem value)
        {
            if (this.ContainsReference(item))
                return;

            var index = this.EnsureSize();
            this.Repository[index] = new KeyValue<TKey, TItem>(item, value);
        }

        public int IndexAndValueOf(TKey key, out TItem value)
        {
            value = default;
            var length = this.Length;
            for (var i = 0; i < length; i++)
            {
                ref var item = ref Repository[i];
                if (ReferenceEquals(item.Key, key))
                {
                    value = item.Value;
                    return i;
                }
            }

            return -1;
        }

        public bool ContainsReference(TKey key)
        {
            var length = this.Length;
            if (length == 1) return ReferenceEquals(this.Repository[0].Key, key);

            for (var i = 0; i < length; i++)
            {
                ref var item = ref Repository[i];
                if (ReferenceEquals(item.Key, key))
                    return true;

            }

            return false;
        }
    }
}
