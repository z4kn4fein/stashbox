#if NET45 || NET40 || NETSTANDARD1_3
using Stashbox.BuildUp.Expressions.Compile.Emitters;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Stashbox.BuildUp.Expressions.Compile
{
    public static class ExpressionEmitter
    {
        private static int methodCounter;

        public static bool TryEmit(this LambdaExpression expression, out Delegate resultDelegate) =>
            TryEmit(expression.Body, out resultDelegate, expression.Type, expression.ReturnType,
                expression.Parameters.ToArray());

        public static bool TryEmitDebug(this LambdaExpression expression, out Delegate resultDelegate) =>
            TryEmitDebug(expression.Body, out resultDelegate, expression.Type, expression.ReturnType,
                expression.Parameters.ToArray());

        public static bool TryEmit(this Expression expression, out Delegate resultDelegate, Type delegateType,
            Type returnType, params ParameterExpression[] parameters)
        {
            resultDelegate = null;

            var analyzer = new TreeAnalyzer();
            if (!analyzer.Analyze(expression, parameters))
                return false;

            var module = new DynamicModule();

            var capturedArgumentsType = analyzer.CapturedParameters.Length > 0 ? module.GetOrAddCapturedArgumentsType(analyzer.CapturedParameters) : null;

            var targetType = analyzer.StoredExpressions.Length > 0 ? module.GetOrAddTargetType(analyzer.StoredExpressions) : null;
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

        public static bool TryEmitDebug(this Expression expression, out Delegate resultDelegate, Type delegateType,
            Type returnType, params ParameterExpression[] parameters)
        {
            resultDelegate = null;

            var analyzer = new TreeAnalyzer();
            if (!analyzer.Analyze(expression, parameters))
                return false;

            var module = new DynamicModule();

            var fields = analyzer.StoredExpressions.Length > 0
                ? module.GetOrAddTargetTypeDebug(analyzer.StoredExpressions)
                : null;

            var capturedArgumentsType = analyzer.CapturedParameters.Length > 0 ? module.GetOrAddCapturedArgumentsType(analyzer.CapturedParameters) : null;

            var delegateTarget = analyzer.StoredExpressions.Length > 0
                ? new DelegateTarget(fields, null, null)
                : null;

            var capturedArgumentsHolder = capturedArgumentsType != null
                ? new CapturedArgumentsHolder(capturedArgumentsType.GetTypeInfo().DeclaredFields.CastToArray(), capturedArgumentsType)
                : null;

            var context = new CompilerContext(delegateTarget, analyzer.DefinedVariables, analyzer.StoredExpressions, analyzer.CapturedParameters, analyzer.NestedLambdas, capturedArgumentsHolder);
            
            if (!context.HasClosure)
            {
                var method = new DynamicMethod("main" + Interlocked.Increment(ref methodCounter), returnType, parameters.GetTypes(), typeof(ExpressionEmitter), true);
                var generator = method.GetILGenerator();

                if (context.DefinedVariables.Length > 0)
                    context.LocalBuilders = Emitter.BuildLocals(context.DefinedVariables, generator);

                if (!expression.TryEmit(generator, context, parameters))
                    return false;

                generator.Emit(OpCodes.Ret);

                resultDelegate = method.CreateDelegate(delegateType);

                return true;
            }

            var builder = DynamicModule.DynamicTypeBuilder.Value.DefineMethod("main" + Interlocked.Increment(ref methodCounter), MethodAttributes.Public,
                returnType, parameters.GetTypes());

            var gen = builder.GetILGenerator();

            if (context.DefinedVariables.Length > 0)
                context.LocalBuilders = Emitter.BuildLocals(context.DefinedVariables, gen);

            if (!expression.TryEmit(gen, context, parameters))
                return false;

            gen.Emit(OpCodes.Ret);

#if NETSTANDARD1_3
            var type = DynamicModule.DynamicTypeBuilder.Value.CreateTypeInfo().AsType();
            //resultDelegate = type.GetTypeInfo().DeclaredMethods.First(m => m.Name == "kaki2").CreateDelegate(delegateType);
#else
            var type = DynamicModule.DynamicTypeBuilder.Value.CreateType();
            //resultDelegate = Delegate.CreateDelegate(delegateType, type.GetTypeInfo().DeclaredMethods.First(m => m.Name == "main2"));
#endif

            return true;
        }
    }
}
#endif