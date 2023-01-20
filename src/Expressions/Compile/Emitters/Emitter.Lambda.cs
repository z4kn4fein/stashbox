using Stashbox.Expressions.Compile.Extensions;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Stashbox.Utils;

namespace Stashbox.Expressions.Compile.Emitters;

internal static partial class Emitter
{
    private static bool TryEmit(this LambdaExpression expression, ILGenerator generator, CompilerContext context)
    {
        var lambdaIndex = context.NestedLambdas.IndexAndValueOf(expression, out var lambda);

        if (lambdaIndex == -1 || context.Target == null || lambda == null) return false;

        var lambdaClosureIndex = lambdaIndex + (context.Target.Constants.Length - context.NestedLambdas.Length);

        generator.Emit(OpCodes.Ldarg_0);
        generator.Emit(OpCodes.Ldfld, Closure.ConstantsField);
        generator.EmitInteger(lambdaClosureIndex);
        generator.Emit(OpCodes.Ldelem_Ref);

        if (lambda.UsesCapturedArgument)
        {
            if (context is { IsNestedLambda: false, CapturedArgumentsHolderVariable: { } })
                generator.Emit(OpCodes.Ldloc, context.CapturedArgumentsHolderVariable);
            else
                generator.Emit(OpCodes.Ldarg_1);
        }

        var variables = lambda.ParameterExpressions;
        var nestedParameters = expression.Parameters.CastToArray();

        var nestedContext = context.Clone(variables, true, lambda.UsesCapturedArgument);
        if (nestedContext.Target == null) return false;

        var method = new DynamicMethod(string.Empty,
            expression.ReturnType,
            nestedContext.HasCapturedVariablesArgument
                ? new[] { TypeCache<Closure>.Type, TypeCache<object[]>.Type }.Append(nestedParameters.GetTypes())
                : TypeCache<Closure>.Type.Append(nestedParameters.GetTypes()),
            TypeCache<Closure>.Type,
            true);

        var nestedGenerator = method.GetILGenerator();

        if (variables.Length > 0)
            nestedContext.LocalBuilders = variables.BuildLocals(nestedGenerator);

        if (nestedContext.HasCapturedVariablesArgument)
            nestedGenerator.CopyParametersToCapturedArgumentsIfAny(nestedContext, nestedParameters);

        if (!expression.Body.TryEmit(nestedGenerator, nestedContext, nestedParameters))
            return false;

        nestedGenerator.Emit(OpCodes.Ret);

        if (nestedContext.HasCapturedVariablesArgument)
        {
            var delegateArgs = TypeCache<object[]>.Type
                .Append(nestedParameters.GetTypes())
                .Append(expression.ReturnType);

            var resultDelegate = method.CreateDelegate(Utils.MapDelegateType(delegateArgs),
                nestedContext.Target);

            nestedContext.Target.Constants[lambdaClosureIndex] = resultDelegate;
            generator.EmitMethod(Utils.GetPartialApplicationMethodInfo(delegateArgs));
        }
        else
        {
            var resultDelegate = method.CreateDelegate(expression.Type, nestedContext.Target);
            nestedContext.Target.Constants[lambdaClosureIndex] = resultDelegate;
        }

        return true;
    }
}