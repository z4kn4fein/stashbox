using System.Collections;
using System.Collections.Generic;

namespace Stashbox.Utils
{
    internal class OrderedLinkedStore<TValue> : IEnumerable<TValue>
    {
        public static OrderedLinkedStore<TValue> Empty = new OrderedLinkedStore<TValue>();

        private readonly OrderedLinkedStore<TValue> next;

        private readonly TValue value;

        private OrderedLinkedStore() { }

        private OrderedLinkedStore(TValue value, OrderedLinkedStore<TValue> next)
        {
            this.value = value;
            this.next = next;
        }

        public OrderedLinkedStore<TValue> Add(TValue paramValue)
        {
            return this.next == null ? new OrderedLinkedStore<TValue>(paramValue, Empty) : this.SelfCopy(this.next.Add(paramValue));
        }

        private OrderedLinkedStore<TValue> SelfCopy(OrderedLinkedStore<TValue> nextNode) =>
            new OrderedLinkedStore<TValue>(this.value, nextNode);

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
                if (this.current == null && this.init.next != null)
                    this.current = this.init;
                else if (this.current?.next != null && this.current.next != Empty)
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