#if NET45 || NET40 || IL_EMIT
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this DefaultExpression expression, ILGenerator generator)
        {
            var type = expression.Type;

            if (type == typeof(void))
                return true;

            if (type == typeof(string))
                generator.Emit(OpCodes.Ldnull);
            else if (type == typeof(bool) ||
                     type == typeof(byte) ||
                     type == typeof(char) ||
                     type == typeof(sbyte) ||
                     type == typeof(int) ||
                     type == typeof(uint) ||
                     type == typeof(short) ||
                     type == typeof(ushort))
                generator.Emit(OpCodes.Ldc_I4_0);
            else if (type == typeof(long) ||
                     type == typeof(ulong))
            {
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Conv_I8);
            }
            else if (type == typeof(float))
                generator.Emit(OpCodes.Ldc_R4, default(float));
            else if (type == typeof(double))
                generator.Emit(OpCodes.Ldc_R8, default(double));
            else if (type.GetTypeInfo().IsValueType)
            {
                var lb = generator.DeclareLocal(type);
                generator.Emit(OpCodes.Ldloca, lb);
                generator.Emit(OpCodes.Initobj, type);
                generator.Emit(OpCodes.Ldloc, lb);
            }
            else
                generator.Emit(OpCodes.Ldnull);

            return true;
        }
    }
}
#endif
