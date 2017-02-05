using System;
using System.Reflection;

namespace Stashbox.Entity.Resolution
{
    internal class ResolutionMember
    {
        public ResolutionTarget ResolutionTarget { get; set; }
        public MemberInfo MemberInfo { get; set; }
    }
}
