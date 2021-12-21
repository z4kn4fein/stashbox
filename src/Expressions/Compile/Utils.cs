using System;
using System.Linq;
using System.Reflection;

namespace Stashbox.Expressions.Compile
{
    internal static class Utils
    {
        public static bool IsInPlaceEmittableConstant(Type type, object value) =>
            type.IsPrimitive ||
            type.IsEnum ||
            value is string or Type;


        public static readonly Type ClosureType = typeof(Closure);

        public static readonly Type ObjectArrayType = typeof(object[]);

        public static Type MapDelegateType(Type[] paramTypes)
        {
            return paramTypes.Length switch
            {
                1 => typeof(Func<>).MakeGenericType(paramTypes),
                2 => typeof(Func<,>).MakeGenericType(paramTypes),
                3 => typeof(Func<,,>).MakeGenericType(paramTypes),
                4 => typeof(Func<,,,>).MakeGenericType(paramTypes),
                5 => typeof(Func<,,,,>).MakeGenericType(paramTypes),
                6 => typeof(Func<,,,,,>).MakeGenericType(paramTypes),
                7 => typeof(Func<,,,,,,>).MakeGenericType(paramTypes),
                8 => typeof(Func<,,,,,,,>).MakeGenericType(paramTypes),
                _ => throw new NotSupportedException($"Too many func parameters {paramTypes.Length}.")
            };
        }

        internal static MethodInfo GetPartialApplicationMethodInfo(Type[] types) =>
            PartialApplication.Methods[types.Length - 2].MakeGenericMethod(types);
    }

    internal static class PartialApplication
    {
        public static readonly MethodInfo[] Methods = typeof(PartialApplication)
            .GetMethods().CastToArray();

        public static Func<TR> ApplyPartial<TV, TR>(Func<TV, TR> f, TV v) => () => f(v);
        public static Func<T1, TR> ApplyPartial<TV, T1, TR>(Func<TV, T1, TR> f, TV v) => t1 => f(v, t1);
        public static Func<T1, T2, TR> ApplyPartial<TV, T1, T2, TR>(Func<TV, T1, T2, TR> f, TV v) => (t1, t2) => f(v, t1, t2);
        public static Func<T1, T2, T3, TR> ApplyPartial<TV, T1, T2, T3, TR>(Func<TV, T1, T2, T3, TR> f, TV v) => (t1, t2, t3) => f(v, t1, t2, t3);
        public static Func<T1, T2, T3, T4, TR> ApplyPartial<TV, T1, T2, T3, T4, TR>(Func<TV, T1, T2, T3, T4, TR> f, TV v) => (t1, t2, t3, t4) => f(v, t1, t2, t3, t4);
        public static Func<T1, T2, T3, T4, T5, TR> ApplyPartial<TV, T1, T2, T3, T4, T5, TR>(Func<TV, T1, T2, T3, T4, T5, TR> f, TV v) => (t1, t2, t3, t4, t5) => f(v, t1, t2, t3, t4, t5);
        public static Func<T1, T2, T3, T4, T5, T6, TR> ApplyPartial<TV, T1, T2, T3, T4, T5, T6, TR>(Func<TV, T1, T2, T3, T4, T5, T6, TR> f, TV v) => (t1, t2, t3, t4, t5, t6) => f(v, t1, t2, t3, t4, t5, t6);
    }
}
