using Stashbox.Utils;
using Stashbox.Utils.Data;
using System.Collections.Generic;
using System.Linq.Expressions;

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

        public static T[] Append<T>(this T[] array, T item)
        {
            var count = array.Length;
            if (count == 0)
                return new[] { item };

            var arr = new T[count + 1];
            Array.Copy(array, arr, count);
            arr[count] = item;
            return arr;
        }

        public static bool ContainsReference<TElement>(this TElement[] array, TElement element) where TElement : class =>
            array.GetReferenceIndex(element) != -1;

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

        public static TSource[] SelectButLast<TSource>(this TSource[] source)
        {
            var length = source.Length;
            if (length == 1)
                return Constants.EmptyArray<TSource>();

            var resultLength = length - 1;
            var result = new TSource[resultLength];
            for (var i = 0; i < resultLength; i++)
                result[i] = source[i];

            return result;
        }

        public static TResult LastElement<TResult>(this TResult[] source) => source[source.Length - 1];

        public static ParameterExpression[] AsParameters(this Type[] source)
        {
            var length = source.Length;
            var result = new ParameterExpression[length];

            for (var i = 0; i < length; i++)
                result[i] = source[i].AsParameter();

            return result;
        }

        public static Pair<bool, ParameterExpression>[] AsParameterPairs(this ParameterExpression[] source)
        {
            var length = source.Length;
            var result = new Pair<bool, ParameterExpression>[length];

            for (var i = 0; i < length; i++)
                result[i] = new Pair<bool, ParameterExpression>(false, source[i]);

            return result;
        }

        public static Stashbox.Utils.Data.Stack<TItem> AsStack<TItem>(this IEnumerable<TItem> enumerable) =>
            Stashbox.Utils.Data.Stack<TItem>.FromEnumerable(enumerable);
    }

    internal static class InternalArrayHelper<T>
    {
        public static readonly T[] Empty = new T[0];
    }
}
