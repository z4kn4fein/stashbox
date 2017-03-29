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
            if (this.next == null)
                return new OrderedLinkedStore<TValue>(paramValue, Empty);

            return this.SelfCopy(this.next.Add(paramValue));
        }

        private OrderedLinkedStore<TValue> SelfCopy(OrderedLinkedStore<TValue> next) =>
            new OrderedLinkedStore<TValue>(this.value, next);

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
                else if (current?.next?.next != null)
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
    internal class LinkedStore<TValue> : IEnumerable<TValue>
    {
        public static LinkedStore<TValue> Empty = new LinkedStore<TValue>();

        private readonly LinkedStore<TValue> next;

        private readonly TValue value;
        
        private LinkedStore() { }

        public LinkedStore(TValue value, LinkedStore<TValue> next)
        {
            this.value = value;
            this.next = next;
        }

        public LinkedStore<TValue> Add(TValue paramValue) => new LinkedStore<TValue>(paramValue, this);

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
