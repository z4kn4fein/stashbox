#if NET45 || NET40 || NETSTANDARD1_3
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this BinaryExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            switch (expression.Left.NodeType)
            {
                case ExpressionType.Parameter:
                    if (context.Target == null)
                        return false;

                    var paramIndex = context.ClosureExpressions.GetIndex(expression.Left);
                    if (paramIndex == -1)
                        return false;

                    if (!expression.Right.TryEmit(generator, context, parameters))
                        return false;

                    generator.Emit(OpCodes.Stfld, context.Target.Fields[paramIndex]);

                    return true;

                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression.Left;

                    if (memberExpression.Expression != null && !memberExpression.Expression.TryEmit(generator, context, parameters))
                        return false;

                    if (!expression.Right.TryEmit(generator, context, parameters))
                        return false;

                    return EmitMemberAssign(memberExpression.Member, generator);

                default:
                    return false;
            }
        }
    }
}
#endif