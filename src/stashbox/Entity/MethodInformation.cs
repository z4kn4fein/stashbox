using System.Reflection;

namespace Stashbox.Entity
{
    public class MethodInformation
    {
        public MethodInfo Method { get; set; }

        public TypeInformation[] Parameters { get; set; }
    }
}
