namespace Stashbox.Utils.Data
{
    internal readonly struct KeyValue<TK, TV>
    {
        public readonly TK Key;

        public readonly TV Value;

        public KeyValue(TK k, TV v)
        {
            this.Key = k;
            this.Value = v;
        }
    }
}