#if NET40
using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    internal struct TypeInfo
    {
        public Type Type { get; }

        public TypeInfo(Type type)
        {
            this.Type = type;
        }

        public Assembly Assembly => this.Type.Assembly;
        public IEnumerable<ConstructorInfo> DeclaredConstructors => this.Type.GetConstructors(All ^ BindingFlags.Static);
        public IEnumerable<MethodInfo> DeclaredMethods => this.Type.GetMethods(All);
        public IEnumerable<FieldInfo> DeclaredFields => this.Type.GetFields(All);
        public IEnumerable<PropertyInfo> DeclaredProperties => this.Type.GetProperties(All);
        public IEnumerable<Type> ImplementedInterfaces => this.Type.GetInterfaces();
        public IEnumerable<Attribute> GetCustomAttributes(Type attributeType, bool inherit) => this.Type.GetCustomAttributes(attributeType, inherit).Cast<Attribute>();
        public Type BaseType => this.Type.BaseType;
        public bool IsGenericType => this.Type.IsGenericType;
        public bool IsGenericTypeDefinition => this.Type.IsGenericTypeDefinition;
        public bool ContainsGenericParameters => this.Type.ContainsGenericParameters;
        public int GenericParameterPosition => this.Type.GenericParameterPosition;
        public GenericParameterAttributes GenericParameterAttributes => this.Type.GenericParameterAttributes;
        public Type[] GenericTypeParameters => this.Type.GetGenericArguments();
        public Type[] GenericTypeArguments => this.Type.GetGenericArguments();
        public Type[] GetGenericParameterConstraints() => this.Type.GetGenericParameterConstraints();
        public bool IsClass => this.Type.IsClass;
        public bool IsInterface => this.Type.IsInterface;
        public bool IsValueType => this.Type.IsValueType;
        public bool IsPrimitive => this.Type.IsPrimitive;
        public bool IsArray => this.Type.IsArray;
        public bool IsPublic => this.Type.IsPublic;
        public bool IsNestedPublic => this.Type.IsNestedPublic;
        public Type DeclaringType => this.Type.DeclaringType;
        public bool IsAbstract => this.Type.IsAbstract;
        public bool IsSealed => this.Type.IsSealed;
        public bool IsEnum => this.Type.IsEnum;
        public bool IsAssignableFrom(TypeInfo info) => this.Type.IsAssignableFrom(info.Type);

        private const BindingFlags All =
            BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.DeclaredOnly;
    }
}
#endif