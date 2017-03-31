using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Entity.Resolution
{
    /// <summary>
    /// Represents a resolution memeber.
    /// </summary>
    public class ResolutionMember
    {
        /// <summary>
        /// The expression to set the member.
        /// </summary>
        public Expression Expression { get; set; }

        /// <summary>
        /// The member info.
        /// </summary>
        public MemberInfo MemberInfo { get; set; }
    }
}
