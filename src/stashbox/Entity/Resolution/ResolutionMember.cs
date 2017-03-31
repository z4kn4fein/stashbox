using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Entity.Resolution
{
    public class ResolutionMember
    {
        public Expression Expression { get; set; }
        public MemberInfo MemberInfo { get; set; }
    }
}
