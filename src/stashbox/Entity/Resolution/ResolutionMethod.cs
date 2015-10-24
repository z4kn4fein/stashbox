using System;
using System.Reflection;

namespace Stashbox.Entity.Resolution
{
    public class ResolutionMethod
    {
        public MethodInfo Method { get; set; }

        public ResolutionTarget[] Parameters { get; set; }

        public Action<object, object[]> MethodDelegate { get; set; }
    }
}
