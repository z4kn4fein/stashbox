using System.Collections.Generic;

namespace System.Linq
{
    internal static class EnumerableExpressions
    {
        public static TEnumerable[] CastToArray<TEnumerable>(this IEnumerable<TEnumerable> enumerable) =>
            enumerable is TEnumerable[] ? (TEnumerable[])enumerable : enumerable.ToArray();
    }
}
