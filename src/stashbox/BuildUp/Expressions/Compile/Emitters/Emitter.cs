#if NET45 || NET40 || IL_EMIT
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
                : new DynamicMethod(string.Empty, returnType, context.Target.TargetType.Append(parameters.GetTypes()), context.Target.TargetType, true);
        }

        private static bool TryEmit(this IList<Expression> expressions, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            for (var i = 0; i < expressions.Count; i++)
                if (!expressions[i].TryEmit(generator, context, parameters))
                    return false;

            return true;
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
}
#endif