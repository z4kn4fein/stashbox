using System;

namespace Stashbox.Utils
{
    internal static class NameGenerator
    {
        public static string GetRegistrationName(Type typeTo, string name = null)
        {
            return string.IsNullOrWhiteSpace(name) ? typeTo.Name : name;
        }
    }
}
