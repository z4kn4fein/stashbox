using System.Reflection;
using Stashbox.BuildUp.Expressions;

namespace Stashbox.Entity.Resolution
{
    internal class ResolutionMethod
    {
        public MethodInfo Method { get; set; }

        public InvokeMethod MethodDelegate { get; set; }

        public ResolutionTarget[] Parameters { get; set; }
    }
}
