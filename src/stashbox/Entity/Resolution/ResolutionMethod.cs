using System.Reflection;
using System.Linq.Expressions;

namespace Stashbox.Entity.Resolution
{
    public class ResolutionMethod
    {
        public MethodInfo Method { get; set; }
        public Expression[] Parameters { get; set; }
    }
}
