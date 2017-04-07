using System.Collections.Generic;

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
    }
}
