using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Entity.Resolution
{
    /// <summary>
    /// Represents a resolution constructor
    /// </summary>
    public class ResolutionConstructor
    {
        /// <summary>
        /// The constructor info.
        /// </summary>
        public ConstructorInfo Constructor { get; set; }

        /// <summary>
        /// The parameters of the constructor.
        /// </summary>
        public Expression[] Parameters { get; set; }
    }
}
