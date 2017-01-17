using System;
using System.Linq;

namespace Stashbox.Utils
{
    internal static class NameGenerator
    {
        public static string GetRegistrationName(Type type, string name = null)
        {
            return string.IsNullOrWhiteSpace(name) ? GenerateName(type) : name;
        }

        private static string GenerateName(Type type)
        {
            if (!type.IsConstructedGenericType) return type.FullName;

            var parts = string.Join("+", type.GenericTypeArguments.Select(arg => arg.FullName));
            return type.FullName + parts;
        }
    }
}
