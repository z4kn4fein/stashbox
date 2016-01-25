using System.Reflection;

namespace Stashbox.Entity
{
    /// <summary>
    /// Represents information about a member.
    /// </summary>
    public class MemberInformation
    {
        /// <summary>
        /// Stores the reflected member information.
        /// </summary>
        public MemberInfo MemberInfo { get; set; }

        /// <summary>
        /// Stores the type information about the member.
        /// </summary>
        public TypeInformation TypeInformation { get; set; }
    }
}
