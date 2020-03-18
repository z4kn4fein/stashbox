using Stashbox.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal class ArrayList<TItem> : IEnumerable<TItem>
    {
        public static ArrayList<TItem> Empty => new ArrayList<TItem>();

        private TItem[] internalStore;

        public int Length;

        public ArrayList() { }

        public ArrayList(int minimalCapacity)
        {
            this.internalStore = new TItem[minimalCapacity];
        }

        public ArrayList(ArrayList<TItem> initial)
        {
            this.internalStore = initial.internalStore;
            this.Length = initial.Length;
        }

        public ArrayList(TItem[] initial)
        {
            this.internalStore = initial;
            this.Length = initial.Length;
        }

        public void Add(TItem item)
        {
            var index = this.EnsureSize();
            this.internalStore[index] = item;
        }

        public TItem this[int i] => this.internalStore == null ? default : this.internalStore[i];

        private int EnsureSize()
        {
            if (this.internalStore == null)
                this.internalStore = new TItem[8];

            var newSize = this.Length + 1;
            if (newSize > this.internalStore.Length)
            {
                var newArray = new TItem[this.Length + 8];
                Array.Copy(this.internalStore, newArray, this.Length);
                this.internalStore = newArray;
            }

            this.Length = newSize;
            return this.Length - 1;
        }
        public void Clear()
        {
            this.internalStore = new TItem[8];
            this.Length = 0;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            for (var i = 0; i < this.Length; i++)
                yield return this.internalStore[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    internal class ArrayList<TKey, TItem> : IEnumerable<TItem>
    {
        public static ArrayList<TKey, TItem> Empty => new ArrayList<TKey, TItem>();

        private KeyValue<TKey, TItem>[] internalStore;

        public int Length;

        public ArrayList() { }

        public ArrayList(KeyValue<TKey, TItem>[] initial)
        {
            this.internalStore = initial;
            this.Length = initial.Length;
        }

        public void Add(TKey key, TItem item)
        {
            var index = this.EnsureSize();
            this.internalStore[index] = new KeyValue<TKey, TItem>(key, item);
        }

        public TItem this[int i] => this.internalStore[i].Value;

        internal ArrayList<TKey, TItem> WhereOrDefault(Func<KeyValue<TKey, TItem>, bool> predicate)
        {
            var initial = this.internalStore.Where(predicate).ToArray();
            return initial.Length == 0 ? null : new ArrayList<TKey, TItem>(initial);
        }

        [MethodImpl(Constants.Inline)]
        public TItem GetOrDefault(TKey key)
        {
            var length = this.Length;
            for (var i = 0; i < length; i++)
            {
                var item = this.internalStore[i];
                if (item.Key.Equals(key))
                    return item.Value;
            }

            return default;
        }

        public IEnumerable<KeyValue<TKey, TItem>> Enumerate()
        {
            for (var i = 0; i < this.Length; i++)
                yield return this.internalStore[i];
        }

        public void Clear()
        {
            this.internalStore = new KeyValue<TKey, TItem>[8];
            this.Length = 0;
        }

        private int EnsureSize()
        {
            if (this.internalStore == null)
                this.internalStore = new KeyValue<TKey, TItem>[8];

            var newSize = this.Length + 1;
            if (newSize > this.internalStore.Length)
            {
                var newArray = new KeyValue<TKey, TItem>[this.Length + 8];
                Array.Copy(this.internalStore, newArray, this.Length);
                this.internalStore = newArray;
            }

            this.Length = newSize;
            return this.Length - 1;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            for (var i = 0; i < this.Length; i++)
                yield return this.internalStore[i].Value;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
