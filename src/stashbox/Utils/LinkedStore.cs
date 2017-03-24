using System.Collections;
using System.Collections.Generic;

namespace Stashbox.Utils
{
    internal class LinkedStore<TValue> : IEnumerable<TValue>
    {
        private readonly LinkedStore<TValue> next;

        private readonly TValue value;

        public int Count { get; }

        public LinkedStore(TValue value, LinkedStore<TValue> next, int count)
        {
            this.value = value;
            this.Count = count;
            this.next = next;
        }

        public LinkedStore<TValue> Add(TValue paramValue) => new LinkedStore<TValue>(paramValue, this, this.Count + 1);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        
        public IEnumerator<TValue> GetEnumerator() => new LinkedStoreEnumerator(this);

        private class LinkedStoreEnumerator : IEnumerator<TValue>
        {
            private readonly LinkedStore<TValue> init;
            private LinkedStore<TValue> current;

            public LinkedStoreEnumerator(LinkedStore<TValue> init)
            {
                this.init = init;
            }

            public bool MoveNext()
            {
                if (this.current == null && this.init.next != null)
                    this.current = this.init;
                else if (current?.next != null)
                    this.current = this.current.next;
                else
                    return false;

                return true;
                
            }

            public void Reset() => this.current = this.init;

            public TValue Current => this.current.value;

            object IEnumerator.Current => this.Current;

            public void Dispose()
            { }
        }
    }
}
