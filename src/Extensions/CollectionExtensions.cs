namespace System.Collections.Generic
{
    internal static class CollectionExtensions
    {
        public static TResult? GetOrDefault<TResult>(this IDictionary<byte, object?>? dict, byte key)
        {
            if ((dict?.TryGetValue(key, out var value) ?? false) && value is TResult result)
                return result;

            return default;
        }

        public static object? GetOrDefault(this IDictionary<byte, object?>? dict, byte key)
        {
            if ((dict?.TryGetValue(key, out var value) ?? false))
                return value;

            return default;
        }

        public static bool TryGet(this IDictionary<byte, object?>? dict, byte key, out object? value)
        {
            if (dict?.TryGetValue(key, out var objValue) ?? false)
            {
                value = objValue;
                return true;
            }

            value = default;
            return false;
        }

        public static bool IsOn(this IDictionary<byte, object?>? dict, byte key) => (dict?.ContainsKey(key) ?? false) && dict[key] is bool boolValue && boolValue;
    }
}
