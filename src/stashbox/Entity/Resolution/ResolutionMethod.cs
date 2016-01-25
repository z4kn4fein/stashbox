using Stashbox.BuildUp.DelegateFactory;
using System.Reflection;

namespace Stashbox.Entity.Resolution
{
    internal class ResolutionMethod
    {
        public MethodInfo Method { get; set; }

        public InvokeMethod MethodDelegate { get; set; }
    }
}
