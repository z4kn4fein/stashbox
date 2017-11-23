#if NET45 || NET40 || IL_EMIT
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
                if (context.HasClosure && context.HasCapturedVariablesArgument)
                    index += 2;
                else if (context.HasClosure || context.HasCapturedVariablesArgument)
                    index++;

                if (!context.IsNestedLambda && context.HasCapturedVariablesArgument)
                    index--;

                generator.LoadParameter(index);
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
    }
}
#endif
