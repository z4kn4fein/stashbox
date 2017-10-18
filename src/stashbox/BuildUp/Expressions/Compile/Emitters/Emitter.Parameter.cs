#if NET45 || NET40 || NETSTANDARD1_3
using System.Collections.Generic;
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
                generator.LoadParameter(index + (context.HasClosure && context.HasCapturedVariablesArgument
                    ? 2
                    : context.HasClosure || context.HasCapturedVariablesArgument
                        ? 1
                        : 0));
                return true;
            }

            var constantIndex = context.StoredExpressions.GetIndex(expression);
            if (constantIndex != -1)
            {
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, context.Target.Fields[constantIndex]);
                return true;
            }

            var definedVariableIndex = context.DefinedVariables.GetIndex(expression);
            if (definedVariableIndex != -1)
            {
                generator.Emit(OpCodes.Ldloc, context.LocalBuilders[definedVariableIndex]);
                return true;
            }

            var capturedVariableIndex = context.CapturedArguments.GetIndex(expression);
            if (capturedVariableIndex != -1)
            {
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldfld, context.CapturedArgumentsHolder.Fields[capturedVariableIndex]);
                return true;
            }

            return true;
        }

        public static void LoadParameter(this ILGenerator generator, int index)
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
