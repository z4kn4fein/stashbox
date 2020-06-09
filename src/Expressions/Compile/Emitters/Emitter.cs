#if IL_EMIT
using Stashbox.Expressions.Compile.Extensions;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.Expressions.Compile.Emitters
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
                    return ((LambdaExpression)expression).TryEmit(generator, context);

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
                : new DynamicMethod(string.Empty, returnType, Utils.ClosureType.Append(parameters.GetTypes()), Utils.ClosureType, true);
        }

        private static bool TryEmit(this IList<Expression> expressions, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            var length = expressions.Count;
            for (var i = 0; i < length; i++)
                if (!expressions[i].TryEmit(generator, context, parameters))
                    return false;

            return true;
        }

        internal static ExpandableArray<LocalBuilder> BuildLocals(this IEnumerable<Expression> variables, ILGenerator ilGenerator) =>
            ExpandableArray<LocalBuilder>.FromEnumerable(variables.Select(v => ilGenerator.DeclareLocal(v.Type)));
    }
}
#endif