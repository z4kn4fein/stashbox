#if NET40
using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    internal struct TypeInfo
    {
        private readonly Type type;

        public TypeInfo(Type type)
        {
            this.type = type;
        }

        public Assembly Assembly => this.type.Assembly;
        public IEnumerable<ConstructorInfo> DeclaredConstructors => this.type.GetConstructors(All ^ BindingFlags.Static);
        public IEnumerable<MethodInfo> DeclaredMethods => this.type.GetMethods(All);
        public IEnumerable<FieldInfo> DeclaredFields => this.type.GetFields(All); 
        public IEnumerable<PropertyInfo> DeclaredProperties => this.type.GetProperties(All);
        public IEnumerable<Type> ImplementedInterfaces  => this.type.GetInterfaces();
        public IEnumerable<Attribute> GetCustomAttributes(Type attributeType, bool inherit) => this.type.GetCustomAttributes(attributeType, inherit).Cast<Attribute>();
        public Type BaseType => this.type.BaseType;
        public bool IsGenericType => this.type.IsGenericType; 
        public bool IsGenericTypeDefinition => this.type.IsGenericTypeDefinition; 
        public bool ContainsGenericParameters => this.type.ContainsGenericParameters;
        public int GenericParameterPosition => this.type.GenericParameterPosition;
        public Type[] GenericTypeParameters => this.type.GetGenericArguments(); 
        public Type[] GenericTypeArguments => this.type.GetGenericArguments(); 
        public Type[] GetGenericParameterConstraints() => this.type.GetGenericParameterConstraints();
        public bool IsClass => this.type.IsClass; 
        public bool IsInterface => this.type.IsInterface; 
        public bool IsValueType => this.type.IsValueType; 
        public bool IsPrimitive => this.type.IsPrimitive; 
        public bool IsArray => this.type.IsArray; 
        public bool IsPublic => this.type.IsPublic; 
        public bool IsNestedPublic => this.type.IsNestedPublic; 
        public Type DeclaringType => this.type.DeclaringType; 
        public bool IsAbstract => this.type.IsAbstract; 
        public bool IsSealed => this.type.IsSealed; 
        public bool IsEnum => this.type.IsEnum; 

        private const BindingFlags All =
            BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.DeclaredOnly;
    }
}
#endif