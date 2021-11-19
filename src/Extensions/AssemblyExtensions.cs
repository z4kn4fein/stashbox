using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    internal static class AssemblyExtensions
    {
        public static IEnumerable<Type> CollectTypes(this Assembly assembly) =>
            assembly.ExportedTypes.Concat(assembly.DefinedTypes.Select(typeInfo => typeInfo.AsType())).Distinct();
    }
}
