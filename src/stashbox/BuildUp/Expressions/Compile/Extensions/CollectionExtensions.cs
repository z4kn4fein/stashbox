using System;

namespace Stashbox.BuildUp.Expressions.Compile
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

        public static bool ContainsElement<TElement>(this TElement[] array, TElement element) => array.GetIndex(element) != -1;

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
    }
}
