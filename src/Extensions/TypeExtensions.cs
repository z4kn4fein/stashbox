﻿using Stashbox.Attributes;
using Stashbox.Configuration;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Stashbox;
using DependencyAttribute = Stashbox.Attributes.DependencyAttribute;

namespace System;

internal static class TypeExtensions
{
    public static Type? GetEnumerableType(this Type type)
    {
        if (type.IsArray)
            return type.GetElementType();

        if (type.ImplementsGenericType(TypeCache.EnumerableType) && type != TypeCache<string>.Type && type.GenericTypeArguments.Length == 1)
            return type.GenericTypeArguments[0];

        return null;
    }

    public static bool IsClosedGenericType(this Type type) =>
        type is { IsGenericType: true, IsGenericTypeDefinition: false };

    public static bool IsOpenGenericType(this Type type) =>
        type is { IsGenericType: true, ContainsGenericParameters: true };

    public static ConstructorInfo? GetConstructor(this Type type, params Type[] args) =>
        Array.Find(type.GetConstructors(), c => c.GetParameters().Select(p => p.ParameterType).SequenceEqual(args));

    public static bool IsDisposable(this Type type) =>
        type.Implements(TypeCache<IDisposable>.Type)
#if HAS_ASYNC_DISPOSABLE
         || type.Implements(TypeCache<IAsyncDisposable>.Type)
#endif
    ;

    public static bool IsCompositionRoot(this Type type) =>
        type.Implements(TypeCache<ICompositionRoot>.Type);

    public static bool IsResolvableType(this Type type) =>
        type is { IsAbstract: false, IsInterface: false, IsClass: true } &&
        type != TypeCache<string>.Type &&
        type.GetCustomAttribute<CompilerGeneratedAttribute>() == null;

    public static IEnumerable<Type> GetRegisterableInterfaceTypes(this Type type) =>
        type.GetInterfaces().Where(t =>
                t != TypeCache<IDisposable>.Type
#if HAS_ASYNC_DISPOSABLE
         && t != TypeCache<IAsyncDisposable>.Type
#endif
        );

    public static IEnumerable<Type> GetRegisterableBaseTypes(this Type type)
    {
        var baseType = type.BaseType;
        while (baseType != null && !baseType.IsObjectType())
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }
    }

    public static IEnumerable<Type> GetPossibleDependencyTypes(this Type type)
    {
        var constructorParameterTypes = type.GetConstructors()
            .SelectMany(c => c.GetParameters().Select(p => p.ParameterType));

        var memberTypes = type.GetProperties().Select(p => p.PropertyType)
            .Concat(type.GetFields().Select(f => f.FieldType));

        return constructorParameterTypes.Concat(memberTypes).Distinct();
    }

    public static bool Implements(this Type type, Type interfaceType) =>
        type == interfaceType || (interfaceType.IsOpenGenericType()
            ? type.ImplementsGenericType(interfaceType.GetGenericTypeDefinition())
            : interfaceType.IsAssignableFrom(type));

    public static bool ImplementsWithoutGenericCheck(this Type type, Type interfaceType) =>
        interfaceType.IsAssignableFrom(type);

    private static bool ImplementsGenericType(this Type type, Type genericType) =>
        type.MapsToGenericTypeDefinition(genericType) ||
        type.HasInterfaceThatMapsToGenericTypeDefinition(genericType) ||
        type.BaseType != null && type.BaseType.ImplementsGenericType(genericType);

    private static bool HasInterfaceThatMapsToGenericTypeDefinition(this Type type, Type genericType) =>
        type.GetInterfaces()
            .Where(it => it.IsGenericType)
            .Any(it => it.GetGenericTypeDefinition() == genericType);

    public static bool MapsToGenericTypeDefinition(this Type type, Type genericType) =>
        genericType.IsGenericTypeDefinition
        && type.IsGenericType
        && type.GetGenericTypeDefinition() == genericType;

    public static ConstructorInfo? GetFirstConstructor(this Type type) =>
        type.GetConstructors().FirstOrDefault();

    public static TypeInformation AsTypeInformation(this ParameterInfo parameter,
        Type? declaringType,
        TypeInformation parent,
        ServiceRegistration? serviceRegistration,
        ContainerConfiguration containerConfiguration)
    {
        var customAttributes = parameter.GetCustomAttributes();
        var dependencyName = parameter.GetNameFromDependencyAttribute(containerConfiguration);

        if (serviceRegistration != null)
        {
            var dependencyBindings = serviceRegistration?.Options.GetOrDefault<Dictionary<object, object?>>(RegistrationOption.DependencyBindings);
            if (dependencyBindings != null || containerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled)
            {
                if (dependencyBindings != null && parameter.Name != null && dependencyBindings.TryGetValue(parameter.Name,
                        out var foundNamedDependencyName))
                    dependencyName = foundNamedDependencyName;
                else if (dependencyBindings != null && dependencyBindings.TryGetValue(parameter.ParameterType,
                             out var foundTypedDependencyName))
                    dependencyName = foundTypedDependencyName;
                else if (dependencyName == null &&
                         containerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled)
                    dependencyName = parameter.Name;
            }
        }

        return new TypeInformation(
            parameter.ParameterType,
            declaringType,
            parent,
            dependencyName,
            customAttributes,
            parameter.Name,
            parameter.HasDefaultValue(),
            parameter.DefaultValue,
            parameter.HasDependencyNameAttribute(containerConfiguration),
            null);
    }

    public static TypeInformation AsTypeInformation(this MemberInfo member, 
        ServiceRegistration? serviceRegistration,
        TypeInformation parent,
        ContainerConfiguration containerConfiguration)
    {
        var customAttributes = member.GetCustomAttributes();
        var dependencyName = member.GetNameFromDependencyAttribute(containerConfiguration);

        if (serviceRegistration != null)
        {
            var dependencyBindings = serviceRegistration?.Options.GetOrDefault<Dictionary<object, object?>>(RegistrationOption.DependencyBindings);
            if (dependencyBindings != null || containerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled)
            {
                if (dependencyBindings != null && dependencyBindings.TryGetValue(member.Name, out var foundNamedDependencyName))
                    dependencyName = foundNamedDependencyName;
                else if (dependencyName == null && containerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled)
                    dependencyName = member.Name;
            }
        }

        var type = member is PropertyInfo prop ? prop.PropertyType : ((FieldInfo)member).FieldType;
        return new TypeInformation(
            type,
            member.DeclaringType,
            parent,
            dependencyName,
            customAttributes,
            member.Name,
            false,
            null,
            member.HasDependencyNameAttribute(containerConfiguration),
            null);
    }

    public static MethodInfo[] GetUsableMethods(this Type type)
    {
        var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(method => method.GetInjectionAttribute() != null);
        var baseType = type.BaseType;
        while (baseType != null && !baseType.IsObjectType())
        {
            methods = methods.Concat(baseType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(method => method.GetInjectionAttribute() != null));
            baseType = baseType.BaseType;
        }

        return methods.CastToArray();
    }

    public static MemberInfo[] GetUsableMembers(this Type type,
        ServiceRegistration? serviceRegistration,
        ContainerConfiguration containerConfiguration)
    {
        var autoMemberOptions = serviceRegistration?.Options.GetOrDefault<AutoMemberOptions>(RegistrationOption.AutoMemberOptions);
        var autoMemberInjectionEnabled = containerConfiguration.AutoMemberInjectionEnabled || autoMemberOptions != null;
        var autoMemberInjectionRule = autoMemberOptions?.AutoMemberInjectionRule ?? containerConfiguration.AutoMemberInjectionRule;

        var publicPropsEnabled = autoMemberInjectionEnabled && (autoMemberInjectionRule & Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter) == Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter;
        var limitedPropsEnabled = autoMemberInjectionEnabled && (autoMemberInjectionRule & Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess) == Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess;
        var fieldsEnabled = autoMemberInjectionEnabled && (autoMemberInjectionRule & Rules.AutoMemberInjectionRules.PrivateFields) == Rules.AutoMemberInjectionRules.PrivateFields;

        var dependencyBindings = serviceRegistration?.Options.GetOrDefault<Dictionary<object, object?>>(RegistrationOption.DependencyBindings);

        var globalRequiredInjectionEnabled = containerConfiguration.RequiredMemberInjectionEnabled;
        var regRequiredInjectionEnabled = serviceRegistration?.Options.GetOrDefault<bool?>(RegistrationOption.RequiredMemberInjectionEnabled);
        var requiredInjectionEnabled = regRequiredInjectionEnabled ?? globalRequiredInjectionEnabled;
        
        IEnumerable<MemberInfo> properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Where(member => member.FilterProperty(dependencyBindings, autoMemberOptions, containerConfiguration, publicPropsEnabled, limitedPropsEnabled, requiredInjectionEnabled));
        IEnumerable<MemberInfo> fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Where(member => member.FilterField(dependencyBindings, autoMemberOptions,  containerConfiguration, fieldsEnabled, requiredInjectionEnabled));

        var baseType = type.BaseType;
        while (baseType != null && !baseType.IsObjectType())
        {
            properties = properties.Concat(baseType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(member => member.FilterProperty(dependencyBindings, autoMemberOptions, containerConfiguration, publicPropsEnabled, limitedPropsEnabled, requiredInjectionEnabled)));
            fields = fields.Concat(baseType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(member => member.FilterField(dependencyBindings, autoMemberOptions, containerConfiguration, fieldsEnabled, requiredInjectionEnabled)));
            baseType = baseType.BaseType;
        }

        return properties.Concat(fields).CastToArray();
    }

    public static bool SatisfiesGenericConstraintsOf(this Type implementationType, Type serviceType)
    {
        if (!implementationType.IsGenericTypeDefinition) return true;

        var serviceParameters = serviceType.GetGenericArguments();
        var serviceParametersLength = serviceParameters.Length;
        var implementationParameters = implementationType.GetGenericArguments();
        var implementationParametersLength = implementationParameters.Length;

        for (var i = 0; i < implementationParametersLength; i++)
        {
            var implementationParameter = implementationParameters[i];
            var parameterPosition = implementationParameter.GenericParameterPosition;
            if (parameterPosition >= serviceParametersLength)
                return false;

            var argumentToValidate = serviceParameters[parameterPosition];
            var parameterAttributes = implementationParameter.GenericParameterAttributes;

            if (parameterAttributes.HasDefaultConstructorConstraint() &&
                !parameterAttributes.HasNotNullableValueTypeConstraint() &&
                !argumentToValidate.IsPrimitive &&
                !argumentToValidate.HasPublicParameterlessConstructor())
                return false;

            if (parameterAttributes.HasReferenceTypeConstraint() &&
                argumentToValidate is { IsClass: false, IsInterface: false })
                return false;
            
            if (parameterAttributes.HasNotNullableValueTypeConstraint() &&
                argumentToValidate is { IsValueType: false, IsPrimitive: false, IsEnum: false })
                return false;

            var constraints = implementationParameter.GetGenericParameterConstraints();
            var constraintsLength = constraints.Length;

            if (constraints.Length <= 0) continue;

            var found = false;
            for (var j = 0; !found && j < constraintsLength; j++)
            {
                var constraintForCheck = constraints[j];
                if (argumentToValidate.Implements(constraintForCheck))
                    found = true;
            }

            return found;
        }

        return true;
    }

    private static bool HasDependencyNameAttribute(this ParameterInfo parameterInfo, ContainerConfiguration containerConfiguration) =>
        parameterInfo.GetCustomAttributes(TypeCache<DependencyNameAttribute>.Type, false).FirstOrDefault() != null ||
        parameterInfo.CustomAttributes.Any(ca => containerConfiguration.AdditionalDependencyNameAttributeTypes != null && 
                                                 containerConfiguration.AdditionalDependencyNameAttributeTypes.Contains(ca.AttributeType));

    private static bool HasDependencyNameAttribute(this MemberInfo memberInfo, ContainerConfiguration containerConfiguration) =>
        memberInfo.GetCustomAttributes(TypeCache<DependencyNameAttribute>.Type, false).FirstOrDefault() != null || 
        memberInfo.CustomAttributes.Any(ca => containerConfiguration.AdditionalDependencyNameAttributeTypes != null && 
                                              containerConfiguration.AdditionalDependencyNameAttributeTypes.Contains(ca.AttributeType));
    
    public static bool IsNullableType(this Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == TypeCache.NullableType;

    public static MethodInfo GetMethod(this Delegate @delegate) =>
        @delegate.GetMethodInfo();

    public static bool IsCompiledLambda(this Delegate @delegate) =>
        @delegate.Target != null && @delegate.Target.GetType().FullName == "System.Runtime.CompilerServices.Closure";

    public static string GetDiagnosticsView(this Type type)
    {
        if (!type.IsGenericType) return type.Name;

        var typeName = type.Name;
        var i = typeName.IndexOf('`');
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            typeName = i != -1 ? typeName[..i] : typeName;
#else
        typeName = i != -1 ? typeName.Substring(0, i) : typeName;
#endif

        typeName += "<";
        if (type.IsGenericTypeDefinition)
            typeName += new string(Enumerable.Repeat(',', type.GetGenericArguments().Length - 1).ToArray());
        else
            typeName += string.Join(",", type.GetGenericArguments().Select(a => a.GetDiagnosticsView()));

        typeName += ">";

        return typeName;
    }

    private static bool IsObjectType(this Type type) => type == TypeCache<object>.Type;

    private static bool HasDefaultConstructorConstraint(this GenericParameterAttributes attributes) =>
        (attributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint;

    private static bool HasReferenceTypeConstraint(this GenericParameterAttributes attributes) =>
        (attributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint;
    
    private static bool HasNotNullableValueTypeConstraint(this GenericParameterAttributes attributes) =>
        (attributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint;

    private static bool FilterProperty(this PropertyInfo prop, Dictionary<object, object?>? dependencyBindings, AutoMemberOptions? autoMemberOptions,
        ContainerConfiguration containerConfiguration, bool publicPropsEnabled, bool limitedPropsEnabled, bool requiredInjectionEnabled)
    {
        var valid = prop.CanWrite && !prop.IsIndexer() &&
                    (prop.HasDependencyAttribute(containerConfiguration) || 
#if HAS_REQUIRED
                     (prop.HasRequiredAttribute() && requiredInjectionEnabled) ||
#endif
                     publicPropsEnabled && prop.GetSetMethod() != null || limitedPropsEnabled ||
                     (dependencyBindings != null && dependencyBindings.ContainsKey(prop.Name)));
        
        valid = valid && (containerConfiguration.AutoMemberInjectionFilter == null || containerConfiguration.AutoMemberInjectionFilter(prop));
        valid = valid && (autoMemberOptions?.AutoMemberInjectionFilter == null || autoMemberOptions.AutoMemberInjectionFilter(prop));

        return valid;
    }

    private static bool FilterField(this FieldInfo field, Dictionary<object, object?>? dependencyBindings, AutoMemberOptions? autoMemberOptions,
        ContainerConfiguration containerConfiguration, bool fieldsEnabled, bool requiredInjectionEnabled)
    {
        var valid = !field.IsInitOnly && !field.IsBackingField() &&
                    (field.HasDependencyAttribute(containerConfiguration) || 
#if HAS_REQUIRED
                     (field.HasRequiredAttribute() && requiredInjectionEnabled) ||
#endif
                     fieldsEnabled ||
                     (dependencyBindings != null && dependencyBindings.ContainsKey(field.Name)));

        valid = valid && (containerConfiguration.AutoMemberInjectionFilter == null || containerConfiguration.AutoMemberInjectionFilter(field));
        valid = valid && (autoMemberOptions?.AutoMemberInjectionFilter == null || autoMemberOptions.AutoMemberInjectionFilter(field));

        return valid;
    }

    private static bool IsBackingField(this MemberInfo field) =>
        field.Name[0] == '<';

    private static bool IsIndexer(this PropertyInfo property) =>
        property.GetIndexParameters().Length != 0;

    private static bool HasDefaultValue(this ParameterInfo parameter) =>
        parameter.IsOptional;

    private static bool HasPublicParameterlessConstructor(this Type type) =>
        Array.Find(type.GetConstructors(), c => c.GetParameters().Length == 0) != null;

    private static object? GetNameFromDependencyAttribute(this MemberInfo property, ContainerConfiguration containerConfiguration)
    {
        var attr = property.GetCustomAttributes(TypeCache<DependencyAttribute>.Type, false).FirstOrDefault();
        if (attr != null) return (attr as DependencyAttribute)?.Name;

        var msAttr = property.CustomAttributes.FirstOrDefault(a => containerConfiguration.AdditionalDependencyAttributeTypes != null &&
                                                                   containerConfiguration.AdditionalDependencyAttributeTypes.Contains(a.AttributeType));
        return msAttr?.ConstructorArguments.FirstOrDefault().Value;
    }

    private static object? GetNameFromDependencyAttribute(this ParameterInfo parameter, ContainerConfiguration containerConfiguration)
    {
        var attr = parameter.GetCustomAttributes(TypeCache<DependencyAttribute>.Type, false).FirstOrDefault();
        if (attr != null) return (attr as DependencyAttribute)?.Name;

        var msAttr = parameter.CustomAttributes.FirstOrDefault(a => containerConfiguration.AdditionalDependencyAttributeTypes != null &&
                                                                    containerConfiguration.AdditionalDependencyAttributeTypes.Contains(a.AttributeType));
        return msAttr?.ConstructorArguments.FirstOrDefault().Value;
    }

    private static bool HasDependencyAttribute(this MemberInfo property, ContainerConfiguration containerConfiguration)
    {
        return property.GetCustomAttributes(TypeCache<DependencyAttribute>.Type, false).FirstOrDefault() != null ||
               property.CustomAttributes.Any(a => containerConfiguration.AdditionalDependencyAttributeTypes != null &&
                                                  containerConfiguration.AdditionalDependencyAttributeTypes.Contains(a.AttributeType));
    }
    
#if HAS_REQUIRED
    private static bool HasRequiredAttribute(this MemberInfo memberInfo) =>
        memberInfo.GetCustomAttributes(TypeCache<RequiredMemberAttribute>.Type, false).FirstOrDefault() != null;
#endif

    private static InjectionMethodAttribute? GetInjectionAttribute(this MemberInfo method)
    {
        var attr = method.GetCustomAttributes(TypeCache<InjectionMethodAttribute>.Type, false).FirstOrDefault();
        return attr as InjectionMethodAttribute;
    }
}