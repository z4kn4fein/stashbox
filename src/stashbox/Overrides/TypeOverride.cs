
using System;
namespace Stashbox.Overrides
{
    /// <summary>
    /// Represents a type override.
    /// </summary>
    public class TypeOverride : Override
    {
        /// <summary>
        /// The override type.
        /// </summary>
        public Type OverrideType { get; private set; }

        /// <summary>
        /// Constructs a <see cref="TypeOverride"/>
        /// </summary>
        /// <param name="type">The override type.</param>
        /// <param name="value">The overridden value.</param>
        public TypeOverride(Type type, object value)
            : base(value)
        {
            this.OverrideType = type;
        }
    }
}
