using System.Reflection;
using System.Linq.Expressions;

namespace Stashbox.Entity.Resolution
{
    /// <summary>
    /// Represents a resolution method.
    /// </summary>
    public class ResolutionMethod
    {
        /// <summary>
        /// The method info.
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// The parameter expressions.
        /// </summary>
        public Expression[] Parameters { get; set; }
    }
}
