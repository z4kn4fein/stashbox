#if NET45 || NET40 || NETSTANDARD1_3
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Stashbox.BuildUp.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this LambdaExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            var lambdaClosureIndex = context.StoredExpressions.GetIndex(expression);
            if (lambdaClosureIndex == -1) return false;

            var lambdaIndex = context.NestedLambdas.GetIndex(expression);
            if (lambdaIndex == -1) return false;

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, context.Target.Fields[lambdaClosureIndex]);

            if (context.HasCapturedVariablesArgument)
            {
                if (!context.IsNestedLambda)
                    generator.Emit(OpCodes.Ldloc, context.CapturedArgumentsHolderVariable);
                else
                    generator.Emit(OpCodes.Ldarg_1);
            }

            var lambda = context.NestedLambdas[lambdaIndex];

            var lambdaExpression = lambda.Key;
            var variables = lambda.Value;

            var nestedParameters = lambdaExpression.Parameters.CastToArray();

            var nestedContext = context.CreateNew(variables, true);

            var method = context.HasCapturedVariablesArgument
                ? new DynamicMethod(string.Empty, lambdaExpression.ReturnType, context.ConcatDelegateTargetAndCapturedArgumentWithParameter(nestedParameters.GetTypes()), context.Target.TargetType, true)
                : new DynamicMethod(string.Empty, lambdaExpression.ReturnType, context.ConcatDelegateTargetWithParameter(nestedParameters.GetTypes()), context.Target.TargetType, true);

            var nestedGenerator = method.GetILGenerator();

            if (variables.Length > 0)
                nestedContext.LocalBuilders = BuildLocals(variables, nestedGenerator);

            if (nestedContext.HasCapturedVariablesArgument)
                nestedGenerator.CopyParametersToCapturedArgumentsIfAny(nestedContext, nestedParameters);

            if (!lambdaExpression.Body.TryEmit(nestedGenerator, nestedContext, nestedParameters))
                return false;

            nestedGenerator.Emit(OpCodes.Ret);

            if (context.HasCapturedVariablesArgument)
            {
                var delegateArgs = context.ConcatCapturedArgumentWithParameterWithReturnType(
                    nestedParameters.GetTypes(),
                    lambdaExpression.ReturnType);

                var resultDelegate = method.CreateDelegate(Utils.MapDelegateType(delegateArgs),
                    nestedContext.Target.Target);

                nestedContext.Target.Fields[lambdaClosureIndex].SetValue(nestedContext.Target.Target, resultDelegate);

                if (context.HasCapturedVariablesArgument)
                    generator.EmitMethod(Utils.GetMapperMethodInfo(delegateArgs));
            }
            else
            {
                var resultDelegate = method.CreateDelegate(lambdaExpression.Type, nestedContext.Target.Target);
                nestedContext.Target.Fields[lambdaClosureIndex].SetValue(nestedContext.Target.Target, resultDelegate);
            }

            return true;
        }
    }
}
#endif
