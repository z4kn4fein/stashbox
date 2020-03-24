using System.Collections.Generic;

namespace System.Linq
{
    internal static class EnumerableExtensions
    {
        public static TEnumerable[] CastToArray<TEnumerable>(this IEnumerable<TEnumerable> enumerable) =>
            enumerable is TEnumerable[] array ? array : enumerable.ToArray();

        public static TElement[] AddElement<TElement>(this TElement[] array, TElement element)
        {
            if (array == null || array.Length == 0)
                return new[] { element };

            var length = array.Length;
            switch (length)
            {
                case 1:
                    return new[] { array[0], element };
                case 2:
                    return new[] { array[0], array[1], element };
                case 3:
                    return new[] { array[0], array[1], array[2], element };
                case 4:
                    return new[] { array[0], array[1], array[2], array[3], element };
                case 5:
                    return new[] { array[0], array[1], array[2], array[3], array[4], element };
                case 6:
                    return new[] { array[0], array[1], array[2], array[3], array[4], array[5], element };
                default:
                    var newArr = new TElement[length + 1];
                    Array.Copy(array, newArr, length);
                    newArr[length] = element;
                    return newArr;
            }
        }

        public static bool IsEmpty<TEnumerable>(this List<TEnumerable> list) =>
            list.Count == 0;

        public static TEnumerable[] WhereOrDefault<TEnumerable>(this IEnumerable<TEnumerable> enumerable, Func<TEnumerable, bool> predicate)
        {
            var result = enumerable.Where(predicate).CastToArray();
            return result.Length > 0 ? result : null;
        }
    }
}
