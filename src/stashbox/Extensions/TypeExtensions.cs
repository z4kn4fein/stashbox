using System.Collections.Generic;
using System.Reflection;

namespace System
{
    internal static class TypeExtensions
    {
        public static Type GetEnumerableType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsArray)
            {
                return type.GetElementType();
            }

            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return typeInfo.GenericTypeArguments[0];
            }

            return null;
        }
    }
}