using System.Collections.Generic;
using System.Linq;

#if !NET40
using System.Linq;
#endif

namespace System.Reflection
{
    internal static class AssemblyExtensions
    {
        public static IEnumerable<Type> CollectTypes(this Assembly assembly)
        {
#if NET40
            return assembly.GetExportedTypes().Concat(assembly.GetTypes()).Distinct();
#else
            return assembly.ExportedTypes.Concat(assembly.DefinedTypes.Select(typeInfo => typeInfo.AsType())).Distinct();
#endif
        }
    }
}
