#if NET45 || NET40 || NETSTANDARD1_3
using System;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this ParameterExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            var index = parameters.GetIndex(expression);
            if (index != -1)
            {
                generator.LoadParameter(context.HasClosure ? index + 1 : index);
                return true;
            }

            var constantIndex = context.ClosureExpressions.GetIndex(expression);
            if (constantIndex == -1) return false;

            generator.LoadConstantField(context, constantIndex);
            return true;
        }

        private static void LoadParameter(this ILGenerator generator, int index)
        {
            switch (index)
            {
                case 0:
                    generator.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (index <= byte.MaxValue)
                        generator.Emit(OpCodes.Ldarg_S, (byte)index);
                    else
                        generator.Emit(OpCodes.Ldarg, index);
                    break;
            }
        }
    }
}
#endif
