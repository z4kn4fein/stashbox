using System;
using System.Reflection;

namespace Stashbox.Entity.Resolution
{
    public class ResolutionMember
    {
        public ResolutionTarget ResolutionTarget { get; set; }
        public Action<object, object> MemberSetter { get; set; }
        public MemberInfo MemberInfo { get; set; }
    }
}
