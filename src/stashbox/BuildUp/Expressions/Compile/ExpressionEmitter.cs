#if NET45 || NET40 || NETSTANDARD1_3
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Stashbox.BuildUp.Expressions.Compile.Emitters;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal static class ExpressionEmitter
    {
        public static bool TryEmit(this Expression expression, out Delegate resultDelegate, Type delegateType,
            Type returnType, params ParameterExpression[] parameters)
        {
            resultDelegate = null;

            var analyzer = new TreeAnalyzer();
            if (!analyzer.Analyze(expression, parameters))
                return false;

            var targetType = DynamicModule.GetDelegateTargetOrDefault(analyzer.ClosureExpressions);
            var delegateTarget = targetType != null
                ? new DelegateTarget(targetType.GetTypeInfo().DeclaredFields.CastToArray(), targetType, Activator.CreateInstance(targetType, analyzer.ClosureObjects))
                : null;

            var context = new CompilerContext(analyzer.ClosureExpressions, delegateTarget);



#if NET45 || NET40

            //var typ = DynamicModule.ModuleBuilder.Value.DefineType("Stashbox.Dynamic.EmittedType", TypeAttributes.Public);

            //var m = typ.DefineMethod("nojrwonfreonre", MethodAttributes.Public | MethodAttributes.Static, returnType, delegateTarget == null ? parameters.GetTypes() : delegateTarget.Target.GetType().ConcatDelegateTargetParameter(parameters.GetTypes()));
            //var g = m.GetILGenerator();
            //if (!expression.TryEmit(g, context, parameters))
            //    return false;

            //g.Emit(OpCodes.Ret);

            //var t = typ.CreateType();
            //DynamicModule.DynamicAssemblyBuilder.Value.Save("Stashbox.Dynamic.dll");

#endif


            var method = Emitter.CreateDynamicMethod(context, returnType, parameters);
            var generator = method.GetILGenerator();

            if (!expression.TryEmit(generator, context, parameters))
                return false;

            generator.Emit(OpCodes.Ret);

            resultDelegate = context.HasClosure
                ? method.CreateDelegate(delegateType, context.Target.Target)
                : method.CreateDelegate(delegateType);

            return true;
        }
    }
}
#endif