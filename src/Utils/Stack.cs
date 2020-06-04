namespace Stashbox.Utils
{
    internal class Stack<TValue> : ExpandableArray<TValue>
    {
        public TValue Pop()
        {
            if (this.Length == 0)
                return default;

            var result = base.Repository[this.Length - 1];
            base.Repository[--this.Length] = default;
            return result;
        }
    }
}
