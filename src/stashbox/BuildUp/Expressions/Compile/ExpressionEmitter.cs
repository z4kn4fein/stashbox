#if NET45 || NET40 || NETSTANDARD1_3
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Stashbox.BuildUp.Expressions.Compile.Emitters;

namespace Stashbox.BuildUp.Expressions.Compile
{
    public static class ExpressionEmitter
    {
        private static int methodCounter;
        
        public static bool TryEmit(this Expression expression, out Delegate resultDelegate, Type delegateType,
            Type returnType, params ParameterExpression[] parameters)
        {
            resultDelegate = null;

            var analyzer = new TreeAnalyzer();
            if (!analyzer.Analyze(expression, parameters))
                return false;

            var module = new DynamicModule();
            
            var fields = analyzer.StoredExpressions.Length > 0
                ? module.GetOrAddTargetType(analyzer.StoredExpressions)
                : null;
            //var targetType = analyzer.StoredExpressions.Length > 0 ? module.GetOrAddTargetType(analyzer.StoredExpressions) : null;
            var capturedArgumentsType = analyzer.CapturedParameters.Length > 0 ? module.GetOrAddCapturedArgumentsType(analyzer.CapturedParameters) : null;
            //var delegateTarget = targetType != null
            //    ? new DelegateTarget(targetType.GetTypeInfo().DeclaredFields.CastToArray(), targetType, Activator.CreateInstance(targetType, analyzer.StoredObjects))
            //    : null;

            var delegateTarget = analyzer.StoredExpressions.Length > 0
                ? new DelegateTarget(fields)
                : null;

            var capturedArgumentsHolder = capturedArgumentsType != null
                ? new CapturedArgumentsHolder(capturedArgumentsType.GetTypeInfo().DeclaredFields.CastToArray(), capturedArgumentsType)
                : null;

            var context = new CompilerContext(delegateTarget, analyzer.DefinedVariables, analyzer.StoredExpressions, analyzer.CapturedParameters, capturedArgumentsHolder);

            if (analyzer.NestedLambdas.Length > 0 && !TryConstructNestedLambdas(analyzer, context))
                return false;

            if (!context.HasClosure)
            {
                var method = new DynamicMethod("main" + Interlocked.Increment(ref methodCounter), returnType, parameters.GetTypes(), typeof(ExpressionEmitter), true);
                var generator = method.GetILGenerator();

                if (context.DefinedVariables.Length > 0)
                    context.LocalBuilders = BuildLocals(context.DefinedVariables, generator);

                if (!expression.TryEmit(generator, context, parameters))
                    return false;

                generator.Emit(OpCodes.Ret);
                
                return true;
            }

            var builder = DynamicModule.DynamicTypeBuilder.Value.DefineMethod("main" + Interlocked.Increment(ref methodCounter), MethodAttributes.Public,
                returnType, parameters.GetTypes());

            var gen = builder.GetILGenerator();

            if (context.DefinedVariables.Length > 0)
                context.LocalBuilders = BuildLocals(context.DefinedVariables, gen);

            if (!expression.TryEmit(gen, context, parameters))
                return false;

            gen.Emit(OpCodes.Ret);

#if NETSTANDARD1_3
            var type = DynamicModule.DynamicTypeBuilder.Value.CreateTypeInfo().AsType();
            resultDelegate = type.GetTypeInfo().DeclaredMethods.First(m => m.Name == "kaki2").CreateDelegate(delegateType);
#else
            var type = DynamicModule.DynamicTypeBuilder.Value.CreateType();
            resultDelegate = Delegate.CreateDelegate(delegateType, type.GetTypeInfo().DeclaredMethods.First(m => m.Name == "main2"));
#endif

            return true;
        }

        private static bool TryConstructNestedLambdas(TreeAnalyzer tree, CompilerContext context)
        {
            var length = tree.NestedLambdas.Length;
            for (var i = 0; i < length; i++)
            {
                var lambda = tree.NestedLambdas[i];

                var lambdaExpression = lambda.Key;
                var variables = lambda.Value;

                var lambdaIndex = tree.NestedLambdas.GetIndex(lambdaExpression);
                if (lambdaIndex == -1)
                    return false;

                var nestedParameters = lambdaExpression.Parameters.CastToArray();

                var nestedContext = context.CreateNew(variables, tree.CapturedParameters.Length > 0);

                if (!context.HasClosure)
                {
                    var method = new DynamicMethod("nested" + Interlocked.Increment(ref methodCounter), lambdaExpression.ReturnType, nestedParameters.GetTypes(), typeof(ExpressionEmitter), true);
                    var generator = method.GetILGenerator();

                    if (variables.Length > 0)
                        nestedContext.LocalBuilders = BuildLocals(variables, generator);

                    if (!lambdaExpression.TryEmit(generator, nestedContext, nestedParameters))
                        return false;

                    generator.Emit(OpCodes.Ret);

                    return true;
                }

                var builder = DynamicModule.DynamicTypeBuilder.Value.DefineMethod("nested" + Interlocked.Increment(ref methodCounter), MethodAttributes.Public,
                    lambdaExpression.ReturnType, context.ConcatCapturedArgumentWithParameter(nestedParameters.GetTypes()));

                var gen = builder.GetILGenerator();

                if (variables.Length > 0)
                    nestedContext.LocalBuilders = BuildLocals(variables, gen);

                if (!lambdaExpression.Body.TryEmit(gen, nestedContext, nestedParameters))
                    return false;

                gen.Emit(OpCodes.Ret);

                //var @delegate = context.HasClosure
                //    ? nestedMethod.CreateDelegate(lambdaExpression.Type, context.Target.Target)
                //    : nestedMethod.CreateDelegate(lambdaExpression.Type);

                //context.Target.Fields[lambdaIndex].SetValue(context.Target.Target, @delegate);
            }

            return true;
        }

        private static LocalBuilder[] BuildLocals(Expression[] variables, ILGenerator ilGenerator)
        {
            var length = variables.Length;
            var locals = new LocalBuilder[length];

            for (var i = 0; i < length; i++)
                locals[i] = ilGenerator.DeclareLocal(variables[i].Type);

            return locals;
        }
    }
}
#endif