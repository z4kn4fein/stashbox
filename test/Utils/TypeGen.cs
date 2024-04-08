using System;
using System.Collections.Generic;

namespace Stashbox.Tests.Utils;

public static class TypeGen
{
    public static (Type, Type) GetCollidingTypes()
    {
        var hashes = new Dictionary<int, Type>();
        uint n = 0;
        while (true)
        {
            var type = GenerateType(n++);
            var hash = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(type);
            if (hashes.TryGetValue(hash, out var hash1))
            {
                return (hash1, type);
            }

            hashes.Add(hash, type);
        }
    }

    private static Type GenerateType(uint n)
    {
        var current = typeof(Nil);
        for (var i=0; i < sizeof(uint) * 8; i++)
        {
            var leading = n >> i;
            if (leading == 0) {
                return current;
            }

            var bit = (n >> i) & 0x01;
            current = bit == 0 ? typeof(Zero<>).MakeGenericType(current) : typeof(One<>).MakeGenericType(current);
        }
        return current;
    }
    
    private class Nil;
    private class Zero<T>;
    private class One<T>;
}