using Stashbox.Expressions.Compile.Extensions;
using Stashbox.Utils;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.Expressions.Compile.Emitters;

internal static partial class Emitter
{
    private static bool TryEmit(this DefaultExpression expression, ILGenerator generator)
    {
        var type = expression.Type;

        if (type == TypeCache.VoidType)
            return true;

        if (type == TypeCache<string>.Type)
            generator.Emit(OpCodes.Ldnull);
        else if (type == TypeCache<bool>.Type ||
                 type == TypeCache<byte>.Type ||
                 type == TypeCache<char>.Type ||
                 type == TypeCache<sbyte>.Type ||
                 type == TypeCache<int>.Type ||
                 type == TypeCache<uint>.Type ||
                 type == TypeCache<short>.Type ||
                 type == TypeCache<ushort>.Type)
            generator.Emit(OpCodes.Ldc_I4_0);
        else if (type == TypeCache<long>.Type ||
                 type == TypeCache<ulong>.Type)
        {
            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Conv_I8);
        }
        else if (type == TypeCache<float>.Type)
            generator.Emit(OpCodes.Ldc_R4, default(float));
        else if (type == TypeCache<double>.Type)
            generator.Emit(OpCodes.Ldc_R8, default(double));
        else if (type.IsValueType)
            generator.InitValueType(type);
        else
            generator.Emit(OpCodes.Ldnull);

        return true;
    }
}