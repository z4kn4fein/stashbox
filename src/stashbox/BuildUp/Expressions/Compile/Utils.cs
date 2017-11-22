using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal static class Utils
    {
        public static Type MapDelegateType(Type[] paramTypes)
        {
            switch (paramTypes.Length)
            {
                case 1: return typeof(Func<>).MakeGenericType(paramTypes);
                case 2: return typeof(Func<,>).MakeGenericType(paramTypes);
                case 3: return typeof(Func<,,>).MakeGenericType(paramTypes);
                case 4: return typeof(Func<,,,>).MakeGenericType(paramTypes);
                case 5: return typeof(Func<,,,,>).MakeGenericType(paramTypes);
                case 6: return typeof(Func<,,,,,>).MakeGenericType(paramTypes);
                case 7: return typeof(Func<,,,,,,>).MakeGenericType(paramTypes);
                case 8: return typeof(Func<,,,,,,,>).MakeGenericType(paramTypes);
                default:
                    throw new NotSupportedException($"Too many func parameters {paramTypes.Length}.");
            }
        }

        internal static MethodInfo GetMapperMethodInfo(Type[] types) =>
            MapperFuncs.Methods[types.Length - 2].MakeGenericMethod(types);
    }

    internal static class MapperFuncs
    {
        private static readonly IEnumerable<MethodInfo> methods =
            typeof(MapperFuncs).GetTypeInfo().DeclaredMethods;

        public static readonly MethodInfo[] Methods = methods.CastToArray();

        public static Func<R> Map<V, R>(Func<V, R> f, V v) { return () => f(v); }
        public static Func<T1, R> Map<V, T1, R>(Func<V, T1, R> f, V v) { return t1 => f(v, t1); }
        public static Func<T1, T2, R> Map<V, T1, T2, R>(Func<V, T1, T2, R> f, V v) { return (t1, t2) => f(v, t1, t2); }
        public static Func<T1, T2, T3, R> Map<V, T1, T2, T3, R>(Func<V, T1, T2, T3, R> f, V v) { return (t1, t2, t3) => f(v, t1, t2, t3); }
        public static Func<T1, T2, T3, T4, R> Map<V, T1, T2, T3, T4, R>(Func<V, T1, T2, T3, T4, R> f, V v) { return (t1, t2, t3, t4) => f(v, t1, t2, t3, t4); }
        public static Func<T1, T2, T3, T4, T5, R> Map<V, T1, T2, T3, T4, T5, R>(Func<V, T1, T2, T3, T4, T5, R> f, V v) { return (t1, t2, t3, t4, t5) => f(v, t1, t2, t3, t4, t5); }
        public static Func<T1, T2, T3, T4, T5, T6, R> Map<V, T1, T2, T3, T4, T5, T6, R>(Func<V, T1, T2, T3, T4, T5, T6, R> f, V v) { return (t1, t2, t3, t4, t5, t6) => f(v, t1, t2, t3, t4, t5, t6); }
    }
}
