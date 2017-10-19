#if NET45 || NET40 || NETSTANDARD1_3
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        public static bool TryEmit(this Expression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Call:
                    return ((MethodCallExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.Invoke:
                    return ((InvocationExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.Parameter:
                    return ((ParameterExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.Lambda:
                    return ((LambdaExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.Convert:
                    return ((UnaryExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.Constant:
                    return ((ConstantExpression)expression).TryEmit(generator, context);

                case ExpressionType.MemberInit:
                    return ((MemberInitExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.New:
                    return ((NewExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.Block:
                    return ((BlockExpression)expression).Expressions.TryEmit(generator, context, parameters);

                case ExpressionType.NewArrayInit:
                    return ((NewArrayExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.Assign:
                    return ((BinaryExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.Default:
                    return ((DefaultExpression)expression).TryEmit(generator);
            }

            return false;
        }
        
        public static DynamicMethod CreateDynamicMethod(CompilerContext context, Type returnType, params ParameterExpression[] parameters)
        {
            return !context.HasClosure
                ? new DynamicMethod(string.Empty, returnType, parameters.GetTypes(), typeof(ExpressionEmitter), true)
                : new DynamicMethod(string.Empty, returnType, context.ConcatDelegateTargetWithParameter(parameters.GetTypes()), context.Target.TargetType, true);
        }

        private static bool TryEmit(this IList<Expression> expressions, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            for (var i = 0; i < expressions.Count; i++)
                if (!expressions[i].TryEmit(generator, context, parameters))
                    return false;

            return true;
        }
        
        internal static MethodInfo GetCurryClosureMethod(Type[] types, CompilerContext context)
        {
            return CurryClosureFuncs.Methods[types.Length - 2].MakeGenericMethod(types);
        }

        internal static LocalBuilder[] BuildLocals(Expression[] variables, ILGenerator ilGenerator)
        {
            var length = variables.Length;
            var locals = new LocalBuilder[length];

            for (var i = 0; i < length; i++)
                locals[i] = ilGenerator.DeclareLocal(variables[i].Type);

            return locals;
        }
    }

    internal static class CurryClosureFuncs
    {
        private static readonly IEnumerable<MethodInfo> _methods =
            typeof(CurryClosureFuncs).GetTypeInfo().DeclaredMethods;

        public static readonly MethodInfo[] Methods = _methods as MethodInfo[] ?? _methods.ToArray();

        public static Func<R> Curry<C, R>(Func<C, R> f, C c) { return () => f(c); }
        public static Func<T1, R> Curry<C, T1, R>(Func<C, T1, R> f, C c) { return t1 => f(c, t1); }
        public static Func<T1, T2, R> Curry<C, T1, T2, R>(Func<C, T1, T2, R> f, C c) { return (t1, t2) => f(c, t1, t2); }
        public static Func<T1, T2, T3, R> Curry<C, T1, T2, T3, R>(Func<C, T1, T2, T3, R> f, C c) { return (t1, t2, t3) => f(c, t1, t2, t3); }
        public static Func<T1, T2, T3, T4, R> Curry<C, T1, T2, T3, T4, R>(Func<C, T1, T2, T3, T4, R> f, C c) { return (t1, t2, t3, t4) => f(c, t1, t2, t3, t4); }
        public static Func<T1, T2, T3, T4, T5, R> Curry<C, T1, T2, T3, T4, T5, R>(Func<C, T1, T2, T3, T4, T5, R> f, C c) { return (t1, t2, t3, t4, t5) => f(c, t1, t2, t3, t4, t5); }
        public static Func<T1, T2, T3, T4, T5, T6, R> Curry<C, T1, T2, T3, T4, T5, T6, R>(Func<C, T1, T2, T3, T4, T5, T6, R> f, C c) { return (t1, t2, t3, t4, t5, t6) => f(c, t1, t2, t3, t4, t5, t6); }
    }
}
#endif