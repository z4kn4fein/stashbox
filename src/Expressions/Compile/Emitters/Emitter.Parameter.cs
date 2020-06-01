#if IL_EMIT
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Stashbox.Expressions.Compile.Extensions;

namespace Stashbox.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this ParameterExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            var index = parameters.GetReferenceIndex(expression);
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

            var definedVariableIndex = context.DefinedVariables.IndexOfReference(expression);
            if (definedVariableIndex != -1)
            {
                generator.Emit(OpCodes.Ldloc, context.LocalBuilders[definedVariableIndex]);
                return true;
            }

            var capturedVariableIndex = context.CapturedArguments.IndexOfReference(expression);
            if (capturedVariableIndex == -1) return true;

            generator.Emit(OpCodes.Ldarg_1);
            generator.EmitInteger(capturedVariableIndex);
            generator.Emit(OpCodes.Ldelem_Ref);
            if (expression.Type.GetTypeInfo().IsValueType)
                generator.Emit(OpCodes.Unbox_Any, expression.Type);
            return true;
        }
    }
}
#endif
