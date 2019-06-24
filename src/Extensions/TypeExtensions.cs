using Stashbox.Attributes;
using Stashbox.BuildUp.Resolution;
using Stashbox.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DependencyAttribute = Stashbox.Attributes.DependencyAttribute;

#if NET40
namespace System.Reflection
{
    internal static class TypeExtensions
    {
        public static TypeInfo GetTypeInfo(this Type type)
        {
            return new TypeInfo(type);
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this TypeInfo typeInfo) where TAttribute : Attribute
        {
            var attrType = typeof(TAttribute);
            var attributes = typeInfo.GetCustomAttributes(attrType, false);
            return (TAttribute)attributes.FirstOrDefault();
        }

        public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo parameter) =>
            parameter.GetCustomAttributes(false).Cast<Attribute>();

        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo property) =>
            property.GetCustomAttributes(false).Cast<Attribute>();
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
            return typeInfo.IsGenericType && !typeInfo.IsGenericTypeDefinition;
        }

        public static bool IsOpenGenericType(this Type type) =>
            type.GetTypeInfo().IsOpenGenericType();

        public static bool IsOpenGenericType(this TypeInfo typeInfo) =>
            typeInfo.IsGenericType && typeInfo.ContainsGenericParameters;

        public static DependencyAttribute GetDependencyAttribute(this MemberInfo property)
        {
            var attr = property.GetCustomAttributes(Constants.DependencyAttributeType, false)?.FirstOrDefault();
            return (DependencyAttribute)attr;
        }

        public static DependencyAttribute GetDependencyAttribute(this ParameterInfo parameter)
        {
            var attr = parameter.GetCustomAttributes(Constants.DependencyAttributeType, false)?.FirstOrDefault();
            return (DependencyAttribute)attr;
        }

        public static InjectionMethodAttribute GetInjectionAttribute(this MemberInfo method)
        {
            var attr = method.GetCustomAttributes(Constants.InjectionAttributeType, false)?.FirstOrDefault();
            return (InjectionMethodAttribute)attr;
        }

        public static Type[] GetGenericArguments(this Type type) =>
            type.GetTypeInfo().GenericTypeArguments;

        public static ConstructorInfo GetConstructor(this Type type, params Type[] args) =>
            type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(c => c.GetParameters().Select(p => p.ParameterType).SequenceEqual(args));

        public static bool IsBackingField(this MemberInfo field) =>
            field.Name[0] == '<';

        public static bool IsIndexer(this PropertyInfo property) =>
            property.GetIndexParameters().Length != 0;

        public static bool HasDefaultValue(this ParameterInfo parameter) =>
            parameter.IsOptional;

        public static bool HasPublicParameterlessConstructor(this TypeInfo info) =>
            info.DeclaredConstructors.FirstOrDefault(c => c.IsPublic && c.GetParameters().Length == 0) != null;

        public static MethodInfo GetSingleMethod(this Type type, string name, bool includeNonPublic = false)
        {
            var found = type.GetSingleMethodOrDefault(name, includeNonPublic);
            if (found == null)
                throw new InvalidOperationException($"'{name}' method not found on {type.FullName}.");

            return found;
        }

        public static MethodInfo GetSingleMethodOrDefault(this Type type, string name, bool includeNonPublic = false) =>
            type.GetTypeInfo().DeclaredMethods.FirstOrDefault(method => (includeNonPublic || method.IsPublic) && method.Name == name);

        public static bool HasSetMethod(this MemberInfo property, bool includeNonPublic = false) =>
            property.GetSetterMethodOrDefault(includeNonPublic) != null;

        public static MethodInfo GetSetterMethodOrDefault(this MemberInfo property, bool includeNonPublic = false) =>
            property.DeclaringType.GetSingleMethodOrDefault("set_" + property.Name, includeNonPublic);

        public static MethodInfo GetGetterMethodOrDefault(this MemberInfo property, bool includeNonPublic = false) =>
            property.DeclaringType.GetSingleMethodOrDefault("get_" + property.Name, includeNonPublic);

        public static bool IsDisposable(this Type type) =>
            type.Implements(Constants.DisposableType);

        public static bool IsCompositionRoot(this Type type) =>
            type.Implements(Constants.CompositionRootType);

        public static bool IsResolvableType(this Type type)
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
            type.GetTypeInfo().Implements(interfaceType);

        public static bool Implements(this TypeInfo typeInfo, Type interfaceType) =>
            interfaceType.GetTypeInfo().IsAssignableFrom(typeInfo);

        public static bool IsFuncType(this Type type) =>
            type.IsClosedGenericType() && FuncResolver.SupportedTypes.Contains(type.GetGenericTypeDefinition());

        public static ConstructorInfo GetFirstConstructor(this Type type) =>
            type.GetTypeInfo().DeclaredConstructors.FirstOrDefault();

        public static ConstructorInfo GetConstructorByTypes(this Type type, params Type[] types)
        {
            if (types.Length == 0)
                return type.GetConstructor(Constants.EmptyTypes);

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

        public static bool IsNullableType(this Type type) =>
            type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static MethodInfo GetMethod(this Delegate @delegate) =>
#if NET40
            @delegate.Method;
#else
            @delegate.GetMethodInfo();
#endif

        public static bool IsCompiledLambda(this Delegate @delegate) =>
            @delegate.Target != null && @delegate.Target.GetType().FullName == "System.Runtime.CompilerServices.Closure";

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