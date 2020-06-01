#if IL_EMIT
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Stashbox.Expressions.Compile.Emitters;
using Stashbox.Expressions.Compile.Extensions;

namespace Stashbox.Expressions.Compile
{
    internal static class ExpressionEmitter
    {
        public static bool TryEmit(this LambdaExpression expression, out Delegate resultDelegate) =>
            TryEmit(expression.Body, out resultDelegate, expression.Type, expression.ReturnType,
                expression.Parameters.ToArray());

        public static bool TryEmit(this Expression expression, out Delegate resultDelegate, Type delegateType,
            Type returnType, params ParameterExpression[] parameters)
        {
            resultDelegate = null;

            var analyzer = new TreeAnalyzer();
            if (!analyzer.Analyze(expression, parameters))
                return false;

            if (!analyzer.NestedLambdas.IsEmpty)
                analyzer.StoredObjects.AddRange(new object[analyzer.NestedLambdas.Length]);

            var closure = analyzer.StoredObjects.IsEmpty
                ? null
                : new Closure(analyzer.StoredObjects.AsArray());

            var context = new CompilerContext(closure,
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
}
#endif