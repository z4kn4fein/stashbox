using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stashbox.BuildUp.Resolution;

namespace System
{
    internal static class TypeExtensions
    {
        public static Type GetEnumerableType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsArray)
                return type.GetElementType();

            if (EnumerableResolver.IsAssignableToGenericType(type, typeof(IEnumerable<>)) && type != typeof(string))
                return typeInfo.GenericTypeArguments[0];

            return null;
        }

        public static bool IsClosedGenericType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsGenericType && !typeInfo.ContainsGenericParameters;
        }

        public static bool IsOpenGenericType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsGenericType && typeInfo.ContainsGenericParameters;
        }
        
        public static IEnumerable<ConstructorInfo> GetAllConstructors(this Type type)
        {
            return type.GetTypeInfo().DeclaredConstructors.Where(c => !c.IsStatic);
        }
        
        public static ConstructorInfo GetConstructor(this Type type, params Type[] args)
        {
            return type.GetAllConstructors().FirstOrDefault(c => c.GetParameters().Select(p => p.ParameterType).SequenceEqual(args));
        }
    }
}