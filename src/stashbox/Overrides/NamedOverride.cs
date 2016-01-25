namespace Stashbox.Overrides
{
    /// <summary>
    /// Represents a named override.
    /// </summary>
    public class NamedOverride : Override
    {
        /// <summary>
        /// The name of the override.
        /// </summary>
        public string OverrideName { get; private set; }

        /// <summary>
        /// Constructs a <see cref="NamedOverride"/>
        /// </summary>
        /// <param name="name">The name of the override.</param>
        /// <param name="value">The overridden value.</param>
        public NamedOverride(string name, object value)
            : base(value)
        {
            this.OverrideName = name;
        }
    }
}