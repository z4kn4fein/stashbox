using System.Collections.Generic;

namespace System.Linq
{
    internal static class EnumerableExtensions
    {
        public static TEnumerable[] CastToArray<TEnumerable>(this IEnumerable<TEnumerable> enumerable) =>
            enumerable is TEnumerable[] array ? array : enumerable.ToArray();

        public static TEnumerable[] WhereOrDefault<TEnumerable>(this IEnumerable<TEnumerable> enumerable, Func<TEnumerable, bool> predicate)
        {
            var result = enumerable.Where(predicate).CastToArray();
            return result.Length > 0 ? result : null;
        }

        public static int GetReferenceIndex<TElement>(this TElement[] array, TElement element) where TElement : class
        {
            if (array == null || array.Length == 0) return -1;

            var length = array.Length;
            if (length == 1) return ReferenceEquals(array[0], element) ? 0 : -1;

            for (var i = 0; i < length; i++)
                if (ReferenceEquals(array[i], element))
                    return i;

            return -1;
        }

        public static bool ContainsReference<TElement>(this TElement[] array, TElement element) where TElement : class =>
            array.GetReferenceIndex(element) != -1;

        public static IEnumerable<TResult> SelectButLast<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            using (var iterator = source.GetEnumerator())
            {
                var isFirst = true;
                var item = default(TSource);

                while (iterator.MoveNext())
                {
                    if (!isFirst) yield return selector(item);
                    item = iterator.Current;
                    isFirst = false;
                }
            }
        }
    }

    internal static class InternalArrayHelper<T>
    {
        public static readonly T[] Empty = new T[0];
    }
}
