#if NET45 || NET40 || IL_EMIT
using Stashbox.BuildUp.Expressions.Compile.Emitters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile
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

            var capturedArgumentsType = analyzer.CapturedParameters.Length > 0 ? DynamicModule.GetOrAddCapturedArgumentsType(analyzer.CapturedParameters) : null;

            if (analyzer.NestedLambdas.Length > 0)
                PrepareNestedLambdaTypes(analyzer, capturedArgumentsType);

            var targetType = analyzer.StoredExpressions.Length > 0 ? DynamicModule.GetOrAddTargetType(analyzer.StoredObjectTypes) : null;

            var delegateTarget = targetType != null
                ? new DelegateTarget(targetType.GetTypeInfo().DeclaredFields.CastToArray(), targetType, Activator.CreateInstance(targetType, analyzer.StoredObjects))
                : null;

            var capturedArgumentsHolder = capturedArgumentsType != null
                ? new CapturedArgumentsHolder(capturedArgumentsType.GetTypeInfo().DeclaredFields.CastToArray(), capturedArgumentsType)
                : null;

            var context = new CompilerContext(delegateTarget, analyzer.DefinedVariables, analyzer.StoredExpressions,
                analyzer.CapturedParameters, analyzer.NestedLambdas, capturedArgumentsHolder);

            var method = Emitter.CreateDynamicMethod(context, returnType, parameters);
            var generator = method.GetILGenerator();

            if (context.HasCapturedVariablesArgument)
            {
                context.CapturedArgumentsHolderVariable = generator.PrepareCapturedArgumentsHolderVariable(capturedArgumentsType);
                generator.CopyParametersToCapturedArgumentsIfAny(context, parameters);
            }

            if (analyzer.DefinedVariables.Length > 0)
                context.LocalBuilders = Emitter.BuildLocals(analyzer.DefinedVariables, generator);

            if (!expression.TryEmit(generator, context, parameters))
                return false;

            generator.Emit(OpCodes.Ret);

            resultDelegate = context.HasClosure
                ? method.CreateDelegate(delegateType, context.Target.Target)
                : method.CreateDelegate(delegateType);

            return true;
        }

        private static void PrepareNestedLambdaTypes(TreeAnalyzer analyzer, Type capturedArgumentType)
        {
            var length = analyzer.NestedLambdas.Length;
            for (var i = 0; i < length; i++)
            {
                var lambda = analyzer.NestedLambdas[i];
                if (capturedArgumentType != null)
                {
                    var type = Utils.MapDelegateType(capturedArgumentType
                        .Append(lambda.Key.Parameters.GetTypes())
                        .Append(lambda.Key.ReturnType));
                    analyzer.AddStoredItem(lambda.Key, type);
                }
                else
                    analyzer.AddStoredItem(lambda.Key, lambda.Key.Type);
            }
        }
    }
}
#endif