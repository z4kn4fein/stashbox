using System.Reflection;

namespace Stashbox.Entity.Resolution
{
    internal class ResolutionConstructor
    {
        public ConstructorInfo Constructor { get; set; }

        public ResolutionTarget[] Parameters { get; set; }
    }
}
