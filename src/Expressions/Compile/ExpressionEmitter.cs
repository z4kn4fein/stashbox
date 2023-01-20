using Stashbox.Expressions.Compile.Emitters;
using Stashbox.Expressions.Compile.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.Expressions.Compile;

internal static class ExpressionEmitter
{
    public static bool TryEmit(this LambdaExpression expression, out Delegate? resultDelegate) =>
        TryEmit(expression.Body, out resultDelegate, expression.Type, expression.ReturnType,
            expression.Parameters.ToArray());

    public static bool TryEmit(this Expression expression, out Delegate? resultDelegate, Type delegateType,
        Type returnType, params ParameterExpression[] parameters)
    {
        resultDelegate = null;

        var analyzer = new TreeAnalyzer();
        if (!analyzer.Analyze(expression, parameters))
            return false;

        var storedObjects = analyzer.Constants.AsArray();

        if (analyzer.NestedLambdas.Length > 0)
            storedObjects = storedObjects.Append(new object[analyzer.NestedLambdas.Length]);

        var closure = storedObjects.Length == 0
            ? null
            : new Closure(storedObjects);

        var context = new CompilerContext(closure,
            analyzer.Constants,
            analyzer.DefinedVariables,
            analyzer.CapturedParameters,
            analyzer.NestedLambdas);

        var method = Emitter.CreateDynamicMethod(context, returnType, parameters);
        var generator = method.GetILGenerator();

        if (context.HasCapturedVariablesArgument)
        {
            context.CapturedArgumentsHolderVariable = generator.PrepareCapturedArgumentsHolderVariable(analyzer.CapturedParameters.Length);
            generator.CopyParametersToCapturedArgumentsIfAny(context, parameters);
        }

        if (analyzer.DefinedVariables.Length > 0)
            context.LocalBuilders = analyzer.DefinedVariables.BuildLocals(generator);

        if (!expression.TryEmit(generator, context, parameters))
            return false;

        generator.Emit(OpCodes.Ret);

        resultDelegate = context.HasClosure
            ? method.CreateDelegate(delegateType, context.Target)
            : method.CreateDelegate(delegateType);

        return true;
    }
}