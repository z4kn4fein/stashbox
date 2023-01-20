using Stashbox.Registration;

namespace System.Collections.Generic;

internal static class CollectionExtensions
{
    public static TResult? GetOrDefault<TResult>(this Dictionary<RegistrationOption, object?>? dict, RegistrationOption key)
    {
        if ((dict?.TryGetValue(key, out var value) ?? false) && value is TResult result)
            return result;

        return default;
    }

    public static object? GetOrDefault(this Dictionary<RegistrationOption, object?>? dict, RegistrationOption key)
    {
        if ((dict?.TryGetValue(key, out var value) ?? false))
            return value;

        return default;
    }

    public static bool TryGet(this Dictionary<RegistrationOption, object?>? dict, RegistrationOption key, out object? value)
    {
        if (dict?.TryGetValue(key, out var objValue) ?? false)
        {
            value = objValue;
            return true;
        }

        value = default;
        return false;
    }

    public static bool IsOn(this Dictionary<RegistrationOption, object?>? dict, RegistrationOption key) => (dict?.ContainsKey(key) ?? false) && dict[key] is true;
}