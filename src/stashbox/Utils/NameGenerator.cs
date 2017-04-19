using System;
using System.Linq;

namespace Stashbox.Utils
{
    internal static class NameGenerator
    {
        public static string GetRegistrationName(Type typeFrom, Type typeTo, string name = null)
        {
            return string.IsNullOrWhiteSpace(name) ? GenerateName(typeTo) : name;
        }

        private static string GenerateName(Type typeTo)
        {
            if (!typeTo.IsClosedGenericType()) return typeTo.Name;

            var parts = string.Join("+", typeTo.GetGenericArguments().Select(arg => arg.Name));
            return typeTo.Name + parts;
        }
    }
}
