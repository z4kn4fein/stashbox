using Stashbox.Infrastructure.Resolution;

namespace Stashbox.Entity.Resolution
{
    /// <summary>
    /// Represents a resolution target.
    /// </summary>
    public class ResolutionTarget
    {
        /// <summary>
        /// Type information about the target.
        /// </summary>
        public TypeInformation TypeInformation { get; set; }

        /// <summary>
        /// Storing a resolver for the target.
        /// </summary>
        public Resolver Resolver { get; set; }

        /// <summary>
        /// Storing an already set value for the target.
        /// </summary>
        public object ResolutionTargetValue { get; set; }

        /// <summary>
        /// Validates the resolution target.
        /// </summary>
        /// <returns>True if it's valid, otherwise false.</returns>
        public bool IsValid() => this.Resolver != null || this.ResolutionTargetValue != null;
    }
}
