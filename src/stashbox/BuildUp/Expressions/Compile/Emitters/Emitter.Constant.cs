#if NET45 || NET40 || IL_EMIT
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this ConstantExpression expression, ILGenerator generator, CompilerContext context)
        {
            var value = expression.Value;
            if (value == null)
            {
                generator.Emit(OpCodes.Ldnull);
                return true;
            }

            var type = expression.Type;

            if (type == typeof(int))
                generator.EmitInteger((int)value);
            else if (type == typeof(char))
                generator.EmitInteger((char)value);
            else if (type == typeof(short))
                generator.EmitInteger((short)value);
            else if (type == typeof(byte))
                generator.EmitInteger((byte)value);
            else if (type == typeof(ushort))
                generator.EmitInteger((ushort)value);
            else if (type == typeof(sbyte))
                generator.EmitInteger((sbyte)value);
            else if (type == typeof(long))
                generator.Emit(OpCodes.Ldc_I8, (long)value);
            else if (type == typeof(float))
                generator.Emit(OpCodes.Ldc_R8, (float)value);
            else if (type == typeof(double))
                generator.Emit(OpCodes.Ldc_R8, (double)value);
            else if (type == typeof(string))
                generator.Emit(OpCodes.Ldstr, (string)value);
            else if (type == typeof(bool))
                generator.Emit((bool)value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            else if (type == typeof(double))
                generator.Emit(OpCodes.Ldc_R8, (double)value);
            else if (value is Type)
            {
                generator.Emit(OpCodes.Ldtoken, (Type)value);
                generator.Emit(OpCodes.Call, typeof(Type).GetSingleMethod("GetTypeFromHandle"));
            }
            else if (context.HasClosure)
            {
                var constantIndex = context.StoredExpressions.GetIndex(expression);
                if (constantIndex == -1) return false;

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, context.Target.Fields[constantIndex]);
                return true;
            }
            else
                return false;

            return true;
        }
    }
}
#endif