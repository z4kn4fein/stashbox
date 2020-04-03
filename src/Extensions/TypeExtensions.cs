using Stashbox.Attributes;
using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Registration;
using Stashbox.Resolution.Resolvers;
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

            if (IsAssignableToGenericType(type, typeof(IEnumerable<>)) && type != typeof(string) && typeInfo.GenericTypeArguments.Length == 1)
                return typeInfo.GenericTypeArguments[0];

            return null;
        }

        public static bool IsClosedGenericType(this Type type) =>
            type.GetTypeInfo().IsClosedGenericType();

        public static bool IsOpenGenericType(this Type type) =>
            type.GetTypeInfo().IsOpenGenericType();

        public static bool IsClosedGenericType(this TypeInfo typeInfo) =>
            typeInfo.IsGenericType && !typeInfo.IsGenericTypeDefinition;

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

        public static TypeInformation AsTypeInformation(this ParameterInfo parameter,
            Type declaringType,
            RegistrationContext registrationContext,
            ContainerConfiguration containerConfiguration)
        {
            var customAttributes = parameter.GetCustomAttributes();
            var dependencyAttribute = parameter.GetDependencyAttribute();
            var typeInfo = new TypeInformation
            {
                Type = parameter.ParameterType,
                DependencyName = dependencyAttribute?.Name,
                ParentType = declaringType,
                CustomAttributes = customAttributes,
                ParameterOrMemberName = parameter.Name,
                HasDefaultValue = parameter.HasDefaultValue(),
                DefaultValue = parameter.DefaultValue
            };

            if (registrationContext.DependencyBindings.Count == 0 && !containerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled)
                return typeInfo;

            if (registrationContext.DependencyBindings.TryGetValue(typeInfo.ParameterOrMemberName, out var foundNamedDependencyName))
                typeInfo.DependencyName = foundNamedDependencyName;
            else if (registrationContext.DependencyBindings.TryGetValue(typeInfo.Type, out var foundTypedDependencyName))
                typeInfo.DependencyName = foundTypedDependencyName;
            else if (typeInfo.DependencyName == null && containerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled)
                typeInfo.DependencyName = typeInfo.ParameterOrMemberName;

            return typeInfo;
        }

        public static TypeInformation AsTypeInformation(this MemberInfo member,
            Type declaringType,
            RegistrationContext registrationContext,
            ContainerConfiguration containerConfiguration)
        {
            var customAttributes = member.GetCustomAttributes();
            var dependencyAttribute = member.GetDependencyAttribute();
            var isProperty = member is PropertyInfo;
            var typeInfo = new TypeInformation
            {
                Type = isProperty ? ((PropertyInfo)member).PropertyType : ((FieldInfo)member).FieldType,
                DependencyName = dependencyAttribute?.Name,
                ParentType = declaringType,
                CustomAttributes = customAttributes,
                ParameterOrMemberName = member.Name,
                MemberType = isProperty ? MemberType.Property : MemberType.Field
            };

            if (registrationContext.InjectionMemberNames.Count == 0 && !containerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled)
                return typeInfo;

            if (registrationContext.InjectionMemberNames.TryGetValue(typeInfo.ParameterOrMemberName, out var foundNamedDependencyName))
                typeInfo.DependencyName = foundNamedDependencyName;
            else if (typeInfo.DependencyName == null && containerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled)
                typeInfo.DependencyName = typeInfo.ParameterOrMemberName;

            return typeInfo;
        }

        public static ConstructorInfo[] GetUsableConstructors(this TypeInfo typeInfo) =>
            typeInfo.DeclaredConstructors.Where(constructor => !constructor.IsStatic && constructor.IsPublic).CastToArray();

        public static MethodInfo[] GetUsableMethods(this TypeInfo typeInfo) =>
            typeInfo.DeclaredMethods.Where(method => method.GetInjectionAttribute() != null).CastToArray();

        public static MemberInfo[] GetUsableMembers(this TypeInfo typeInfo, RegistrationContext contextData, ContainerConfiguration containerConfiguration)
        {
            var autoMemberInjectionEnabled = containerConfiguration.MemberInjectionWithoutAnnotationEnabled || contextData.AutoMemberInjectionEnabled;
            var autoMemberInjectionRule = contextData.AutoMemberInjectionEnabled ? contextData.AutoMemberInjectionRule :
                containerConfiguration.MemberInjectionWithoutAnnotationRule;

            var publicPropsEnabled = autoMemberInjectionEnabled && (autoMemberInjectionRule & Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter) == Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter;
            var limitedPropsEnabled = autoMemberInjectionEnabled && (autoMemberInjectionRule & Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess) == Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess;
            var fieldsEnbaled = autoMemberInjectionEnabled && (autoMemberInjectionRule & Rules.AutoMemberInjectionRules.PrivateFields) == Rules.AutoMemberInjectionRules.PrivateFields;

            var members = typeInfo.DeclaredMembers.Where(member => member.Filter(contextData, containerConfiguration, publicPropsEnabled, limitedPropsEnabled, fieldsEnbaled));

            var baseType = typeInfo.BaseType;
            while (baseType != null && !baseType.IsObjectType())
            {
                var baseTypeInfo = baseType.GetTypeInfo();
                members = members.Concat(baseTypeInfo.DeclaredMembers.Where(member => member.Filter(contextData, containerConfiguration, publicPropsEnabled, limitedPropsEnabled, fieldsEnbaled)));
                baseType = baseTypeInfo.BaseType;
            }

            return members.CastToArray();
        }

        public static bool SatisfiesGenericConstraintsOf(this Type typeForCheck, TypeInfo against)
        {
            if (!against.IsGenericTypeDefinition) return true;

            var parametersLength = against.GenericTypeParameters.Length;
            var typeForCheckInfo = typeForCheck.GetTypeInfo();
            var argumentsLength = typeForCheckInfo.GenericTypeArguments.Length;

            for (var i = 0; i < parametersLength; i++)
            {
                var paramType = against.GenericTypeParameters[i].GetTypeInfo();
                var parameterPosition = paramType.GenericParameterPosition;
                if (parameterPosition >= argumentsLength)
                    return false;

                var argumentType = typeForCheckInfo.GenericTypeArguments[parameterPosition];
                var argumentForValidation = argumentType.GetTypeInfo();
                var parameterAttributes = paramType.GenericParameterAttributes;

                if (parameterAttributes.HasDefaultConstructorConstraint() &&
                    !argumentForValidation.IsPrimitive &&
                    !argumentForValidation.HasPublicParameterlessConstructor())
                    return false;

                if (parameterAttributes.HasReferenceTypeConstraint() &&
                    !argumentForValidation.IsClass)
                    return false;

                var constraints = paramType.GetGenericParameterConstraints();
                var constraintsLength = constraints.Length;

                if (constraints.Length > 0)
                {
                    var found = false;
                    for (int j = 0; j < constraintsLength; j++)
                    {
                        var con = constraints[j];
                        var constraintForCheck = con.IsClosedGenericType() ? con.GetGenericTypeDefinition().MakeGenericType(argumentType) : con;
                        if (argumentForValidation.Implements(constraintForCheck))
                            found = true;
                    }

                    return found;
                }
            }

            return true;
        }

        public static bool Filter(this MemberInfo member, RegistrationContext contextData, ContainerConfiguration containerConfiguration, bool publicPropsEnabled, bool limitedPropsEnabled, bool fieldsEnbaled)
        {
            bool valid;
            if (member is PropertyInfo prop)
                valid = prop.CanWrite && !prop.IsIndexer() &&
                    (member.GetDependencyAttribute() != null ||
                    publicPropsEnabled && prop.HasSetMethod() || limitedPropsEnabled ||
                    contextData.InjectionMemberNames.ContainsKey(member.Name));

            else if (member is FieldInfo field)
                valid = !field.IsInitOnly && !field.IsBackingField() &&
                    (member.GetDependencyAttribute() != null ||
                    fieldsEnbaled ||
                    contextData.InjectionMemberNames.ContainsKey(member.Name));

            else
                return false;

            valid = valid && (containerConfiguration.MemberInjectionFilter == null || containerConfiguration.MemberInjectionFilter(member));
            valid = valid && (contextData.MemberInjectionFilter == null || contextData.MemberInjectionFilter(member));

            return valid;
        }

        public static bool IsNullableType(this Type type) =>
            type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static bool HasDefaultConstructorConstraint(this GenericParameterAttributes attributes) =>
            (attributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint;

        public static bool HasReferenceTypeConstraint(this GenericParameterAttributes attributes) =>
            (attributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint;

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