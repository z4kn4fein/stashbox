using System.Collections.Generic;
#if !NET40
using System.Linq;
#endif

namespace System.Reflection
{
    internal static class AssemblyExtensions
    {
        public static IEnumerable<Type> CollectExportedTypes(this Assembly assembly)
        {
#if NET40
            return assembly.GetExportedTypes();
#else
            return assembly.ExportedTypes;
#endif
        }

        public static IEnumerable<Type> CollectDefinedTypes(this Assembly assembly)
        {
#if NET40
            return assembly.GetTypes();
#else
            return assembly.DefinedTypes.Select(typeInfo => typeInfo.AsType());
#endif
        }
    }
}
