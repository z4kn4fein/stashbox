using System;

namespace Stashbox.Utils
{
    internal static class NameGenerator
    {
        public static string GetRegistrationName(string name = null)
        {
            return string.IsNullOrWhiteSpace(name) ? Guid.NewGuid().ToString() : name;
        }
    }
}
