namespace Stashbox.Entity
{
    /// <summary>
    /// Represents a key-value reference type.
    /// </summary>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public class KeyValue<TKey, TValue>
    {
        /// <summary>
        /// The key.
        /// </summary>
        public TKey Key { get; set; }

        /// <summary>
        /// The value.
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Constructs a <see cref="KeyValue{TKey,TValue}"/>.
        /// </summary>
        public KeyValue(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
