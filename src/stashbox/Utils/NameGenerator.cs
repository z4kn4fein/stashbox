using System;
using System.Linq;

namespace Stashbox.Utils
{
    internal static class NameGenerator
    {
        public static string GetRegistrationName(Type typeTo, string name = null)
        {
            return string.IsNullOrWhiteSpace(name) ? GenerateName(typeTo) : name;
        }

        private static string GenerateName(Type typeTo)
        {
            if (!typeTo.IsConstructedGenericType) return typeTo.Name;

            var parts = string.Join("+", typeTo.GenericTypeArguments.Select(arg => arg.FullName));
            return typeTo.Name + parts;
        }
    }
}
