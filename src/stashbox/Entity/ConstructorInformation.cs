using System.Collections.Generic;
using System.Reflection;

namespace Stashbox.Entity
{
    public class ConstructorInformation
    {
        public ConstructorInfo Method { get; set; }

        public bool HasInjectionAttribute { get; set; }

        public HashSet<ResolutionTarget> Parameters { get; set; }
    }
}