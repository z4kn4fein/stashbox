using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Stashbox;

#if NET40
namespace System.Reflection
{
    internal static class TypeExtensions
    {
        public static TypeInfo GetTypeInfo(this Type type)
        {
            return new TypeInfo(type);
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this MethodInfo method) where TAttribute : Attribute
        {
            var attrType = typeof(TAttribute);
            var attributes = method.GetCustomAttributes(attrType, false);
            return (TAttribute)attributes.FirstOrDefault();
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this ParameterInfo parameter) where TAttribute : Attribute
        {
            var attrType = typeof(TAttribute);
            var attributes = parameter.GetCustomAttributes(attrType, false);
            return (TAttribute)attributes.FirstOrDefault();
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this PropertyInfo property) where TAttribute : Attribute
        {
            var attrType = typeof(TAttribute);
            var attributes = property.GetCustomAttributes(attrType, false);
            return (TAttribute)attributes.FirstOrDefault();
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this FieldInfo field) where TAttribute : Attribute
        {
            var attrType = typeof(TAttribute);
            var attributes = field.GetCustomAttributes(attrType, false);
            return (TAttribute)attributes.FirstOrDefault();
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this TypeInfo typeInfo) where TAttribute : Attribute
        {
            var attrType = typeof(TAttribute);
            var attributes = typeInfo.GetCustomAttributes(attrType, false);
            return (TAttribute)attributes.FirstOrDefault();
        }

        public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo parameter) =>
            parameter.GetCustomAttributes(false).Cast<Attribute>();

        public static IEnumerable<Attribute> GetCustomAttributes(this PropertyInfo property) =>
            property.GetCustomAttributes(false).Cast<Attribute>();

        public static IEnumerable<Attribute> GetCustomAttributes(this FieldInfo field) =>
            field.GetCustomAttributes(false).Cast<Attribute>();
    }
}
#endif

namespace System
{
    internal static class TypeExtensions
    {
        public static Type GetEnumerableType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsArray)
                return type.GetElementType();

            if (IsAssignableToGenericType(type, typeof(IEnumerable<>)) && type != typeof(string))
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

        public static Type[] GetGenericArguments(this Type type) =>
            type.GetTypeInfo().GenericTypeArguments;

        public static ConstructorInfo GetConstructor(this Type type, params Type[] args) =>
            type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(c => c.GetParameters().Select(p => p.ParameterType).SequenceEqual(args));

        public static bool IsBackingField(this FieldInfo field) =>
            field.Name[0] == '<';

        public static bool IsIndexer(this PropertyInfo property) =>
            property.GetIndexParameters().Length != 0;

        public static bool HasDefaultValue(this ParameterInfo parameter) =>
            parameter.IsOptional;

        public static MethodInfo GetSingleMethod(this Type type, string name, bool includeNonPublic = false)
        {
            var found = type.GetTypeInfo().DeclaredMethods.FirstOrDefault(method => (includeNonPublic || method.IsPublic) && method.Name == name);
            if (found == null)
                throw new InvalidOperationException($"'{name}' method not found on {type.FullName}.");

            return found;
        }

        public static MethodInfo GetSingleMethodOrDefault(this Type type, string name, bool includeNonPublic = false) =>
            type.GetTypeInfo().DeclaredMethods.FirstOrDefault(method => (includeNonPublic || method.IsPublic) && method.Name == name);

        public static bool HasSetMethod(this PropertyInfo property, bool includeNonPublic = false) =>
            property.DeclaringType.GetSingleMethodOrDefault("set_" + property.Name, includeNonPublic) != null;

        public static bool IsDisposable(this Type type) =>
            type.Implements(Constants.DisposableType);

        public static bool IsCompositionRoot(this Type type) =>
            type.Implements(Constants.CompositionRootType);

        public static bool IsValidForRegistration(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return !typeInfo.IsAbstract &&
                !typeInfo.IsInterface &&
                typeInfo.IsClass &&
                type != typeof(string) &&
                typeInfo.GetCustomAttribute<CompilerGeneratedAttribute>() == null;
        }

        public static IEnumerable<Type> GetRegisterableInterfaceTypes(this Type type) =>
            type.GetTypeInfo().ImplementedInterfaces.Where(t => t != Constants.DisposableType);

        public static IEnumerable<Type> GetRegisterableBaseTypes(this Type type)
        {
            var baseType = type.GetTypeInfo().BaseType;
            while (baseType != null && !baseType.IsObjectType())
            {
                yield return baseType;
                baseType = baseType.GetTypeInfo().BaseType;
            }
        }

        public static bool IsObjectType(this Type type) => type == Constants.ObjectType;

        public static bool Implements(this Type type, Type interfaceType) =>
            interfaceType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());

        public static ConstructorInfo GetConstructorByTypes(this Type type, params Type[] types)
        {
            if (types.Length == 0)
                return type.GetConstructor(Constants.EmptyTypes);

            var ctor = type.GetConstructor(types);
            if (ctor != null)
                return ctor;

            return type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(constructor =>
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length != types.Length)
                    return false;

                var length = parameters.Length;
                for (var i = 0; i < length; i++)
                {
                    var paramType = parameters[i].ParameterType;
                    var argType = types[i];

                    var eq = paramType == argType;
                    var im = argType.Implements(paramType);

                    if (!eq && !im)
                        return false;
                }

                return true;
            });
        }

        public static MethodInfo GetMethod(this Delegate @delegate) =>
#if NET40
            @delegate.Method;
#else
            @delegate.GetMethodInfo();
#endif

#if NETSTANDARD1_3
        public static MethodInfo GetGetMethod(this PropertyInfo property) =>
            property.DeclaringType.GetMethod($"get_{property.Name}");

        public static MethodInfo GetSetMethod(this PropertyInfo property) =>
            property.DeclaringType.GetMethod($"set_{property.Name}");

        public static MethodInfo GetPropertyGetMethod(this Type type, string name) =>
            type.GetTypeInfo().DeclaredMethods.Single(method => method.Name == $"get_{name}");

        public static MethodInfo GetMethod(this Type type, string name) =>
            type.GetTypeInfo().DeclaredMethods.Single(method => method.Name == name);
#endif

        private static bool IsAssignableToGenericType(Type type, Type genericType)
        {
            if (type == null || genericType == null) return false;

            return type == genericType
              || MapsToGenericTypeDefinition(type, genericType)
              || HasInterfaceThatMapsToGenericTypeDefinition(type, genericType)
              || IsAssignableToGenericType(type.GetTypeInfo().BaseType, genericType);
        }

        private static bool HasInterfaceThatMapsToGenericTypeDefinition(Type type, Type genericType)
        {
            return type.GetTypeInfo().ImplementedInterfaces
              .Where(it => it.GetTypeInfo().IsGenericType)
              .Any(it => it.GetGenericTypeDefinition() == genericType);
        }

        private static bool MapsToGenericTypeDefinition(Type type, Type genericType)
        {
            return genericType.GetTypeInfo().IsGenericTypeDefinition
              && type.GetTypeInfo().IsGenericType
              && type.GetGenericTypeDefinition() == genericType;
        }
    }
}