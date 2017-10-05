#if NET45 || NET40 || NETSTANDARD1_3
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this Expression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
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

                case ExpressionType.Default:
                    return ((DefaultExpression)expression).TryEmit(generator);
            }

            return false;
        }

        private static bool TryEmit(this IList<Expression> expressions, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            for (var i = 0; i < expressions.Count; i++)
                if (!expressions[i].TryEmit(generator, context, parameters))
                    return false;

            return true;
        }

        private static void EmitInteger(this ILGenerator generator, int intValue)
        {
            switch (intValue)
            {
                case 0:
                    generator.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    generator.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    generator.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    generator.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    generator.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    generator.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    generator.Emit(OpCodes.Ldc_I4, intValue);
                    break;
            }
        }
    }
}
#endif