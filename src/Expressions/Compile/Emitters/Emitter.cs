#if IL_EMIT
using Stashbox.Expressions.Compile.Extensions;
using Stashbox.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        public static bool TryEmit(this Expression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            return expression.NodeType switch
            {
                ExpressionType.Call => ((MethodCallExpression)expression).TryEmit(generator, context, parameters),
                ExpressionType.MemberAccess => ((MemberExpression)expression).TryEmit(generator, context, parameters),
                ExpressionType.Invoke => ((InvocationExpression)expression).TryEmit(generator, context, parameters),
                ExpressionType.Parameter => ((ParameterExpression)expression).TryEmit(generator, context, parameters),
                ExpressionType.Lambda => ((LambdaExpression)expression).TryEmit(generator, context),
                ExpressionType.Convert => ((UnaryExpression)expression).TryEmit(generator, context, parameters),
                ExpressionType.Constant => ((ConstantExpression)expression).TryEmit(generator, context),
                ExpressionType.MemberInit => ((MemberInitExpression)expression).TryEmit(generator, context, parameters),
                ExpressionType.New => ((NewExpression)expression).TryEmit(generator, context, parameters),
                ExpressionType.Block => ((BlockExpression)expression).Expressions.TryEmit(generator, context, parameters),
                ExpressionType.NewArrayInit => ((NewArrayExpression)expression).TryEmit(generator, context, parameters),
                ExpressionType.Assign => ((BinaryExpression)expression).TryEmit(generator, context, parameters),
                ExpressionType.Default => ((DefaultExpression)expression).TryEmit(generator),
                _ => false
            };
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

        internal static LocalBuilder[] BuildLocals(this ExpandableArray<Expression> variables, ILGenerator ilGenerator)
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