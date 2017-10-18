//#define ILDebug
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
        private static int methodCounter;

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
                if (!context.HasCapturedVariablesArgumentConstructed)
                    generator.Emit(OpCodes.Newobj, context.CapturedArgumentsHolder.TargetType.GetTypeInfo().DeclaredConstructors.First());
                else
                    generator.Emit(OpCodes.Ldarg_1);

                var length = context.CapturedArguments.Length;
                for (var i = 0; i < length; i++)
                {
                    var arg = context.CapturedArguments[i];
                    var paramIndex = parameters.GetIndex(arg);
                    if (paramIndex != -1)
                    {
                        generator.Emit(OpCodes.Dup);

                        generator.LoadParameter(paramIndex + 1);
                        generator.Emit(OpCodes.Stfld,
                            context.CapturedArgumentsHolder.Fields[i]);

                        continue;
                    }

                    var definedVariableIndex = context.DefinedVariables.GetIndex(arg);
                    if (definedVariableIndex != -1)
                    {
                        generator.Emit(OpCodes.Dup);

                        generator.Emit(OpCodes.Ldloc, context.LocalBuilders[definedVariableIndex]);
                        generator.Emit(OpCodes.Stfld,
                            context.CapturedArgumentsHolder.Fields[i]);
                    }
                }
            }
#if ILDebug
            var lambda = context.NestedLambdas[lambdaIndex];

            var lambdaExpression = lambda.Key;
            var variables = lambda.Value;
            
            var nestedParameters = lambdaExpression.Parameters.CastToArray();

            var nestedContext = context.CreateNew(variables);
            
            var builder = DynamicModule.DynamicTypeBuilder.Value.DefineMethod("nested" + Interlocked.Increment(ref methodCounter), MethodAttributes.Public,
                lambdaExpression.ReturnType, context.ConcatCapturedArgumentWithParameter(nestedParameters.GetTypes()));

            var gen = builder.GetILGenerator();

            if (variables.Length > 0)
                nestedContext.LocalBuilders = BuildLocals(variables, gen);

            if (!lambdaExpression.Body.TryEmit(gen, nestedContext, nestedParameters))
                return false;

            gen.Emit(OpCodes.Ret);

            if (context.HasCapturedVariablesArgument)
                generator.EmitMethod(GetCurryClosureMethod(context.ConcatCapturedArgumentWithParameterWithReturnType(expression.Parameters.ToArray().GetTypes(), lambdaExpression.ReturnType), context));
#else
            var lambda = context.NestedLambdas[lambdaIndex];

            var lambdaExpression = lambda.Key;
            var variables = lambda.Value;

            var nestedParameters = lambdaExpression.Parameters.CastToArray();

            var nestedContext = context.CreateNew(variables);

            var method = new DynamicMethod(string.Empty, lambdaExpression.ReturnType, context.ConcatDelegateTargetAndCapturedArgumentWithParameter(nestedParameters.GetTypes()), context.Target.TargetType, true);
            var nestedGenerator = method.GetILGenerator();

            if (variables.Length > 0)
                nestedContext.LocalBuilders = BuildLocals(variables, nestedGenerator);

            if (!lambdaExpression.Body.TryEmit(nestedGenerator, nestedContext, nestedParameters))
                return false;

            nestedGenerator.Emit(OpCodes.Ret);

            var delegateArgs = context.ConcatCapturedArgumentWithParameterWithReturnType(expression.Parameters.GetTypes(),
                    lambdaExpression.ReturnType);
            var resultDelegate = method.CreateDelegate(Utils.GetFuncType(delegateArgs), nestedContext.Target.Target);

            nestedContext.Target.Fields[lambdaClosureIndex].SetValue(nestedContext.Target.Target, resultDelegate);

            if (context.HasCapturedVariablesArgument)
                generator.EmitMethod(GetCurryClosureMethod(delegateArgs, context));
#endif
            return true;
        }
    }
}
#endif
