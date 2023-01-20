using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.Expressions.Compile.Extensions;

internal static class CollectionExtensions
{
    public static Type[] GetTypes(this IList<ParameterExpression> parameters)
    {
        var count = parameters.Count;
        switch (count)
        {
            case 0:
                return TypeCache.EmptyTypes;
            case 1:
                return new[] { parameters[0].Type };
        }

        var types = new Type[count];
        for (var i = 0; i < count; i++)
            types[i] = parameters[i].Type;
        return types;
    }

    public static Type[] Append(this Type type, Type[] types)
    {
        var count = types.Length;
        if (count == 0)
            return new[] { type };

        var arr = new Type[count + 1];
        arr[0] = type;
        Array.Copy(types, 0, arr, 1, count);
        return arr;
    }

    public static Type[] Append(this Type[] types, Type type)
    {
        var count = types.Length;
        if (count == 0)
            return new[] { type };

        var arr = new Type[count + 1];
        Array.Copy(types, 0, arr, 0, count);
        arr[count] = type;
        return arr;
    }

    public static TItem[] Append<TItem>(this TItem[] types, TItem[] others)
    {
        if (others.Length == 0)
            return types;

        if (types.Length == 0)
            return others;

        var length = others.Length + types.Length;
        var arr = new TItem[length];
        Array.Copy(types, 0, arr, 0, types.Length);
        Array.Copy(others, 0, arr, types.Length, others.Length);
        return arr;
    }
}