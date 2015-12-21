using System;

namespace Stashbox.Utils
{
    internal static class NameGenerator
    {
        public static string GetRegistrationName(Type type, string name = null)
        {
            return string.IsNullOrWhiteSpace(name) ? type.FullName : name;
        }
    }
}
