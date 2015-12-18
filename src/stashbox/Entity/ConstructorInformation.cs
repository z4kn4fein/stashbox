using System.Reflection;

namespace Stashbox.Entity
{
    public class ConstructorInformation
    {
        public ConstructorInfo Constructor { get; set; }

        public bool HasInjectionAttribute { get; set; }

        public TypeInformation[] Parameters { get; set; }
    }
}