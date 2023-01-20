using System;
using System.Collections.Generic;
using System.Linq;
using Stashbox.Expressions.Compile;

namespace Stashbox.Utils;

internal static class TypeCache<TType>
{
    public static readonly Type Type = typeof(TType);
}

internal static class TypeCache
{
    public static readonly Type FuncType = typeof(Func<>);

    public static readonly Type[] EmptyTypes = EmptyArray<Type>();

    public static readonly Type VoidType = typeof(void);

    public static readonly Type EnumerableType = typeof(IEnumerable<>);

    public static readonly Type NullableType = typeof(Nullable<>);

    public static readonly Type ExpressionEmitterType = typeof(ExpressionEmitter);

    public static T[] EmptyArray<T>() => InternalArrayHelper<T>.Empty;

    private static class InternalArrayHelper<T>
    {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static readonly T[] Empty = Array.Empty<T>();
#else
        public static readonly T[] Empty = new T[0];
#endif
    }
}