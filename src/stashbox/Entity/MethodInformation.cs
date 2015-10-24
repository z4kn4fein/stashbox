using System.Collections.Generic;
using System.Reflection;

namespace Stashbox.Entity
{
    public class MethodInformation
    {
        public MethodInfo Method { get; set; }

        public HashSet<TypeInformation> Parameters { get; set; }
    }
}
