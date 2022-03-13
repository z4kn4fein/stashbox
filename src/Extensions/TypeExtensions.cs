using Stashbox.Attributes;
using Stashbox.Configuration;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DependencyAttribute = Stashbox.Attributes.DependencyAttribute;

namespace System
{
    internal static class TypeExtensions
    {
        public static Type? GetEnumerableType(this Type type)
        {
            if (type.IsArray)
                return type.GetElementType();

            if (type.ImplementsGenericType(typeof(IEnumerable<>)) && type != Constants.StringType && type.GenericTypeArguments.Length == 1)
                return type.GenericTypeArguments[0];

            return null;
        }

        public static bool IsClosedGenericType(this Type type) =>
            type.IsGenericType && !type.IsGenericTypeDefinition;

        public static bool IsOpenGenericType(this Type type) =>
            type.IsGenericType && type.ContainsGenericParameters;

        public static ConstructorInfo? GetConstructor(this Type type, params Type[] args) =>
            type.GetConstructors().FirstOrDefault(c => c.GetParameters().Select(p => p.ParameterType).SequenceEqual(args));

        public static bool IsDisposable(this Type type) =>
            type.Implements(Constants.DisposableType)
#if HAS_ASYNC_DISPOSABLE
         || type.Implements(Constants.AsyncDisposableType)
#endif
        ;

        public static bool IsCompositionRoot(this Type type) =>
            type.Implements(Constants.CompositionRootType);

        public static bool IsResolvableType(this Type type) =>
            !type.IsAbstract &&
                !type.IsInterface &&
                type.IsClass &&
                type != Constants.StringType &&
                type.GetCustomAttribute<CompilerGeneratedAttribute>() == null;

        public static IEnumerable<Type> GetRegisterableInterfaceTypes(this Type type) =>
            type.GetInterfaces().Where(t =>
            t != Constants.DisposableType
#if HAS_ASYNC_DISPOSABLE
         && t != Constants.AsyncDisposableType
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

        public static bool Implements(this Type type, Type interfaceType) =>
            type == interfaceType || (interfaceType.IsOpenGenericType()
                ? type.ImplementsGenericType(interfaceType.GetGenericTypeDefinition())
                : interfaceType.IsAssignableFrom(type));

        public static bool ImplementsWithoutGenericCheck(this Type type, Type interfaceType) =>
            interfaceType.IsAssignableFrom(type);

        private static bool ImplementsGenericType(this Type type, Type genericType) =>
            MapsToGenericTypeDefinition(type, genericType) ||
            HasInterfaceThatMapsToGenericTypeDefinition(type, genericType) ||
            type.BaseType != null && type.BaseType.ImplementsGenericType(genericType);

        private static bool HasInterfaceThatMapsToGenericTypeDefinition(Type type, Type genericType) =>
            type.GetInterfaces()
              .Where(it => it.IsGenericType)
              .Any(it => it.GetGenericTypeDefinition() == genericType);

        private static bool MapsToGenericTypeDefinition(Type type, Type genericType) =>
            genericType.IsGenericTypeDefinition
              && type.IsGenericType
              && type.GetGenericTypeDefinition() == genericType;

        public static ConstructorInfo? GetFirstConstructor(this Type type) =>
            type.GetConstructors().FirstOrDefault();

        public static TypeInformation AsTypeInformation(this ParameterInfo parameter,
            Type? declaringType,
            ServiceRegistration serviceRegistration,
            ContainerConfiguration containerConfiguration)
        {
            var customAttributes = parameter.GetCustomAttributes();
            var dependencyName = parameter.GetDependencyAttribute()?.Name;

            if (serviceRegistration.DependencyBindings != null || containerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled)
            {
                if (serviceRegistration.DependencyBindings != null && parameter.Name != null && serviceRegistration.DependencyBindings.TryGetValue(parameter.Name,
                    out var foundNamedDependencyName))
                    dependencyName = foundNamedDependencyName;
                else if (serviceRegistration.DependencyBindings != null && serviceRegistration.DependencyBindings.TryGetValue(parameter.ParameterType,
                    out var foundTypedDependencyName))
                    dependencyName = foundTypedDependencyName;
                else if (dependencyName == null &&
                         containerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled)
                    dependencyName = parameter.Name;
            }

            return new TypeInformation(
                parameter.ParameterType,
                declaringType,
                dependencyName,
                customAttributes,
                parameter.Name,
                parameter.HasDefaultValue(),
                parameter.DefaultValue,
                null);
        }

        public static TypeInformation AsTypeInformation(this MemberInfo member,
            ServiceRegistration serviceRegistration,
            ContainerConfiguration containerConfiguration)
        {
            var customAttributes = member.GetCustomAttributes();
            var dependencyName = member.GetDependencyAttribute()?.Name;

            if (serviceRegistration.DependencyBindings != null || containerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled)
            {
                if (serviceRegistration.DependencyBindings != null && serviceRegistration.DependencyBindings.TryGetValue(member.Name, out var foundNamedDependencyName))
                    dependencyName = foundNamedDependencyName;
                else if (dependencyName == null && containerConfiguration.TreatingParameterAndMemberNameAsDependencyNameEnabled)
                    dependencyName = member.Name;
            }

            var type = member is PropertyInfo prop ? prop.PropertyType : ((FieldInfo)member).FieldType;
            return new TypeInformation(
                type, 
                member.DeclaringType, 
                dependencyName, 
                customAttributes, 
                member.Name, 
                false, 
                null,
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
            ServiceRegistration serviceRegistration,
            ContainerConfiguration containerConfiguration)
        {
            var autoMemberInjectionEnabled = containerConfiguration.AutoMemberInjectionEnabled || serviceRegistration.AutoMemberInjectionEnabled;
            var autoMemberInjectionRule = serviceRegistration.AutoMemberInjectionEnabled ? serviceRegistration.AutoMemberInjectionRule :
                containerConfiguration.AutoMemberInjectionRule;

            var publicPropsEnabled = autoMemberInjectionEnabled && (autoMemberInjectionRule & Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter) == Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter;
            var limitedPropsEnabled = autoMemberInjectionEnabled && (autoMemberInjectionRule & Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess) == Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess;
            var fieldsEnabled = autoMemberInjectionEnabled && (autoMemberInjectionRule & Rules.AutoMemberInjectionRules.PrivateFields) == Rules.AutoMemberInjectionRules.PrivateFields;

            IEnumerable<MemberInfo> properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(member => member.FilterProperty(serviceRegistration, containerConfiguration, publicPropsEnabled, limitedPropsEnabled));
            IEnumerable<MemberInfo> fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(member => member.FilterField(serviceRegistration, containerConfiguration, fieldsEnabled));

            var baseType = type.BaseType;
            while (baseType != null && !baseType.IsObjectType())
            {
                properties = properties.Concat(baseType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(member => member.FilterProperty(serviceRegistration, containerConfiguration, publicPropsEnabled, limitedPropsEnabled)));
                fields = fields.Concat(baseType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(member => member.FilterField(serviceRegistration, containerConfiguration, fieldsEnabled)));
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
                    !argumentToValidate.IsPrimitive &&
                    !argumentToValidate.HasPublicParameterlessConstructor())
                    return false;

                if (parameterAttributes.HasReferenceTypeConstraint() &&
                    !argumentToValidate.IsClass)
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

        public static bool IsNullableType(this Type type) =>
            type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static MethodInfo GetMethod(this Delegate @delegate) =>
            @delegate.GetMethodInfo();

        public static bool IsCompiledLambda(this Delegate @delegate) =>
            @delegate.Target != null && @delegate.Target.GetType().FullName == "System.Runtime.CompilerServices.Closure";

        public static string GetDiagnosticsView(this Type type)
        {
            if (!type.IsGenericType) return type.Name;
            
            var typeName = type.Name;
            var i = typeName.IndexOf('`');
            typeName = i != -1 ? typeName.Substring(0, i) : typeName;

            typeName += "<";
            if (type.IsGenericTypeDefinition)
                typeName += new string(Enumerable.Repeat(',', type.GetGenericArguments().Length - 1).ToArray());
            else
                typeName += string.Join(",", type.GetGenericArguments().Select(a => a.GetDiagnosticsView()));

            typeName += ">";

            return typeName;
        }
        
        private static bool IsObjectType(this Type type) => type == Constants.ObjectType;

        private static bool HasDefaultConstructorConstraint(this GenericParameterAttributes attributes) =>
            (attributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint;

        private static bool HasReferenceTypeConstraint(this GenericParameterAttributes attributes) =>
            (attributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint;
        
        private static bool FilterProperty(this PropertyInfo prop, ServiceRegistration serviceRegistration,
            ContainerConfiguration containerConfiguration, bool publicPropsEnabled, bool limitedPropsEnabled)
        {
            var valid = prop.CanWrite && !prop.IsIndexer() &&
                    (prop.GetDependencyAttribute() != null ||
                     publicPropsEnabled && prop.GetSetMethod() != null || limitedPropsEnabled ||
                     (serviceRegistration.DependencyBindings != null && serviceRegistration.DependencyBindings.ContainsKey(prop.Name)));

            valid = valid && (containerConfiguration.AutoMemberInjectionFilter == null || containerConfiguration.AutoMemberInjectionFilter(prop));
            valid = valid && (serviceRegistration.AutoMemberInjectionFilter == null || serviceRegistration.AutoMemberInjectionFilter(prop));

            return valid;
        }

        private static bool FilterField(this FieldInfo field, ServiceRegistration serviceRegistration,
            ContainerConfiguration containerConfiguration, bool fieldsEnabled)
        {
            var valid = !field.IsInitOnly && !field.IsBackingField() &&
                        (field.GetDependencyAttribute() != null ||
                         fieldsEnabled ||
                         (serviceRegistration.DependencyBindings != null && serviceRegistration.DependencyBindings.ContainsKey(field.Name)));

            valid = valid && (containerConfiguration.AutoMemberInjectionFilter == null || containerConfiguration.AutoMemberInjectionFilter(field));
            valid = valid && (serviceRegistration.AutoMemberInjectionFilter == null || serviceRegistration.AutoMemberInjectionFilter(field));

            return valid;
        }
        
        private static bool IsBackingField(this MemberInfo field) =>
            field.Name[0] == '<';

        private static bool IsIndexer(this PropertyInfo property) =>
            property.GetIndexParameters().Length != 0;

        private static bool HasDefaultValue(this ParameterInfo parameter) =>
            parameter.IsOptional;

        private static bool HasPublicParameterlessConstructor(this Type type) =>
            type.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 0) != null;
        
        private static DependencyAttribute? GetDependencyAttribute(this MemberInfo property)
        {
            var attr = property.GetCustomAttributes(Constants.DependencyAttributeType, false).FirstOrDefault();
            return attr as DependencyAttribute;
        }

        private static DependencyAttribute? GetDependencyAttribute(this ParameterInfo parameter)
        {
            var attr = parameter.GetCustomAttributes(Constants.DependencyAttributeType, false).FirstOrDefault();
            return attr as DependencyAttribute;
        }

        private static InjectionMethodAttribute? GetInjectionAttribute(this MemberInfo method)
        {
            var attr = method.GetCustomAttributes(Constants.InjectionAttributeType, false).FirstOrDefault();
            return attr as InjectionMethodAttribute;
        }
    }
}