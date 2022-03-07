#nullable disable

namespace Stashbox.Utils.Data.Immutable
{
    internal class ImmutableLinkedList<TValue>
    {
        public static readonly ImmutableLinkedList<TValue> Empty = new();

        public readonly TValue Value;

        public readonly ImmutableLinkedList<TValue> Next;

        private ImmutableLinkedList(ImmutableLinkedList<TValue> next, TValue value)
        {
            this.Value = value;
            this.Next = next;
        }

        private ImmutableLinkedList()
        { }

        public ImmutableLinkedList<TValue> Add(TValue value) =>
            new(this, value);
    }
}
