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
            var index = Array.IndexOf(parameters, expression);
            if (index != -1) return generator.LoadParameter(context.Target == null ? index : index + 1);

            var constantIndex = context.ClosureExpressions.GetIndex(expression);
            return constantIndex != -1 && generator.LoadConstantFromField(context, constantIndex);
        }

        private static bool LoadParameter(this ILGenerator generator, int index)
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

            return true;
        }
    }
}
#endif
