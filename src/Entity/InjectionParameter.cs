namespace Stashbox.Entity
{
    /// <summary>
    /// Represents an injection parameter.
    /// </summary>
    public class InjectionParameter
    {
        /// <summary>
        /// The name of the injection parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the injection parameter.
        /// </summary>
        public object Value { get; set; }
    }
}
