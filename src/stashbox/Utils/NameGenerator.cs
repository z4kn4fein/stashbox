using System;
using System.Linq;

namespace Stashbox.Utils
{
    internal static class NameGenerator
    {
        public static string GetRegistrationName(Type typeFrom, Type typeTo, string name = null)
        {
            return string.IsNullOrWhiteSpace(name) ? GenerateName(typeFrom, typeTo) : name;
        }

        private static string GenerateName(Type typeFrom, Type typeTo)
        {
            if (!typeTo.IsConstructedGenericType) return typeFrom.Name + typeTo.Name;

            var parts = string.Join("+", typeTo.GenericTypeArguments.Select(arg => arg.FullName));
            return typeFrom.Name + typeTo.Name + parts;
        }
    }
}
