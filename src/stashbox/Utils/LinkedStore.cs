using System.Collections;
using System.Collections.Generic;

namespace Stashbox.Utils
{
    internal class OrderedLinkedStore<TValue> : IEnumerable<TValue>
    {
        public static readonly OrderedLinkedStore<TValue> Empty = new OrderedLinkedStore<TValue>();

        public OrderedLinkedStore<TValue> Next;

        public TValue Value;

        public bool IsEmpty;

        private OrderedLinkedStore() { this.IsEmpty = true; }

        private OrderedLinkedStore(TValue value, OrderedLinkedStore<TValue> next)
        {
            this.Value = value;
            this.Next = next;
            this.IsEmpty = false;
        }

        public OrderedLinkedStore<TValue> Add(TValue paramValue)
        {
            return this.Next == null ? new OrderedLinkedStore<TValue>(paramValue, Empty) : this.SelfCopy(this.Next.Add(paramValue));
        }

        private OrderedLinkedStore<TValue> SelfCopy(OrderedLinkedStore<TValue> nextNode) =>
            new OrderedLinkedStore<TValue>(this.Value, nextNode);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public IEnumerator<TValue> GetEnumerator() => new OrderedLinkedStoreEnumerator(this);

        private class OrderedLinkedStoreEnumerator : IEnumerator<TValue>
        {
            private readonly OrderedLinkedStore<TValue> init;
            private OrderedLinkedStore<TValue> current;

            public OrderedLinkedStoreEnumerator(OrderedLinkedStore<TValue> init)
            {
                this.init = init;
            }

            public bool MoveNext()
            {
                if (this.current == null && this.init.Next != null)
                    this.current = this.init;
                else if (this.current?.Next != null && this.current.Next != Empty)
                    this.current = this.current.Next;
                else
                    return false;

                return true;
            }

            public void Reset() => this.current = this.init;

            public TValue Current => this.current.Value;

            object IEnumerator.Current => this.Current;

            public void Dispose()
            {
                // do nothing
            }
        }
    }
}