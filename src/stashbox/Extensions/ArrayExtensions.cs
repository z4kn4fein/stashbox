using System.Collections.ObjectModel;

namespace Stashbox.Extensions
{
    public static class ArrayExtensions
    {
        public static ReadOnlyCollection<TArray> CreateCopy<TArray>(this TArray[] array)
        {
            return new ReadOnlyCollection<TArray>(array);
        }
    }
}
