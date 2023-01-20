using Stashbox.Expressions.Compile.Extensions;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.Expressions.Compile.Emitters;

internal static partial class Emitter
{
    private static bool TryEmit(this ParameterExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
    {
        var index = parameters.GetReferenceIndex(expression);
        if (index != -1)
        {
            if (context is { HasClosure: true, HasCapturedVariablesArgument: true })
                index += 2;
            else if (context.HasClosure || context.HasCapturedVariablesArgument)
                index++;

            if (context is { IsNestedLambda: false, HasCapturedVariablesArgument: true })
                index--;

            generator.LoadParameter(index);
            return true;
        }

        var definedVariableIndex = context.DefinedVariables.IndexOf(expression);
        if (definedVariableIndex != -1 && context.LocalBuilders != null)
        {
            generator.Emit(OpCodes.Ldloc, context.LocalBuilders[definedVariableIndex]);
            return true;
        }

        var capturedVariableIndex = context.CapturedArguments.IndexOf(expression);
        if (capturedVariableIndex == -1) return true;

        generator.Emit(OpCodes.Ldarg_1);
        generator.EmitInteger(capturedVariableIndex);
        generator.Emit(OpCodes.Ldelem_Ref);
        if (expression.Type.IsValueType)
            generator.Emit(OpCodes.Unbox_Any, expression.Type);
        return true;
    }
}