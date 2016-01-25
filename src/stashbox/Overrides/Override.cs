namespace Stashbox.Overrides
{
    /// <summary>
    /// Represents an override.
    /// </summary>
    public class Override
    {
        /// <summary>
        /// The overridden value.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Constructs an <see cref="Override"/>
        /// </summary>
        /// <param name="value">The overridden value.</param>
        public Override(object value)
        {
            this.Value = value;
        }
    }
}