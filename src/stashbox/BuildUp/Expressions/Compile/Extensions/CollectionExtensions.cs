using Stashbox;
using Stashbox.Entity;
using System.Linq.Expressions;

namespace System.Collections.Generic
{
    internal static class CollectionExtensions
    {
        public static int GetIndex<TElement>(this TElement[] array, TElement element)
        {
            if (array == null || array.Length == 0) return -1;

            var length = array.Length;
            if (length == 1) return array[0].Equals(element) ? 0 : -1;

            for (int i = 0; i < length; i++)
                if (array[i].Equals(element))
                    return i;

            return -1;
        }

        public static int GetIndex<TKey, TValue>(this KeyValue<TKey, TValue>[] array, TKey element)
        {
            if (array == null || array.Length == 0) return -1;

            var length = array.Length;
            if (length == 1) return array[0].Key.Equals(element) ? 0 : -1;

            for (int i = 0; i < length; i++)
                if (array[i].Key.Equals(element))
                    return i;

            return -1;
        }

        public static bool ContainsElement<TElement>(this TElement[] array, TElement element) => array.GetIndex(element) != -1;

        public static Type[] GetTypes(this IList<ParameterExpression> parameters)
        {
            var count = parameters.Count;
            if (count == 0)
                return Constants.EmptyTypes;
            if (count == 1)
                return new[] { parameters[0].Type };

            var types = new Type[count];
            for (var i = 0; i < count; i++)
                types[i] = parameters[i].Type;
            return types;
        }

        public static Type[] Append(this Type type, Type[] types)
        {
            var count = types.Length;
            if (count == 0)
                return new[] { type };

            var arr = new Type[count + 1];
            arr[0] = type;
            Array.Copy(types, 0, arr, 1, count);
            return arr;
        }

        public static Type[] Append(this Type[] types, Type type)
        {
            var count = types.Length;
            if (count == 0)
                return new[] { type };

            var arr = new Type[count + 1];
            Array.Copy(types, 0, arr, 0, count);
            arr[count] = type;
            return arr;
        }

        public static Type[] Append(this Type[] types, Type[] others)
        {
            if (others.Length == 0)
                return types;

            if (types.Length == 0)
                return others;

            var length = others.Length + types.Length;
            var arr = new Type[length];
            Array.Copy(types, 0, arr, 0, types.Length);
            Array.Copy(others, 0, arr, types.Length, others.Length);
            return arr;
        }
    }
}
