using System.Diagnostics;

namespace Stashbox
{
    /// <summary>
    /// Represents a readonly key-value pair.
    /// </summary>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    [DebuggerDisplay("{Value}", Name = "{Key}")]
    public readonly struct ReadOnlyKeyValue<TKey, TValue>
    {
        /// <summary>
        /// The key.
        /// </summary>
        public readonly TKey Key;

        /// <summary>
        /// The value.
        /// </summary>
        public readonly TValue Value;

        /// <summary>
        /// Constructs a <see cref="ReadOnlyKeyValue{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public ReadOnlyKeyValue(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}