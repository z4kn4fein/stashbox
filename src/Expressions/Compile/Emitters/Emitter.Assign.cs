using Stashbox.Expressions.Compile.Extensions;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Stashbox.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this BinaryExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            switch (expression.Left.NodeType)
            {
                case ExpressionType.Parameter:

                    var localIndex = context.DefinedVariables.IndexOf(expression.Left);
                    if (localIndex == -1) return false;

                    if (!expression.Right.TryEmit(generator, context, parameters))
                        return false;

                    generator.Emit(OpCodes.Stloc, context.LocalBuilders[localIndex]);

                    if (!context.HasCapturedVariablesArgument) return true;

                    var paramIndex = context.CapturedArguments.IndexOf(expression.Left);
                    if (paramIndex == -1) return true;

                    generator.LoadCapturedArgumentHolder(context);
                    generator.EmitInteger(paramIndex);
                    generator.Emit(OpCodes.Ldloc, context.LocalBuilders[localIndex]);
                    if (expression.Type.IsValueType)
                        generator.Emit(OpCodes.Box, expression.Type);
                    generator.Emit(OpCodes.Stelem_Ref);

                    return true;

                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression.Left;

                    if (memberExpression.Expression != null && !memberExpression.Expression.TryEmit(generator, context, parameters))
                        return false;

                    return expression.Right.TryEmit(generator, context, parameters) && memberExpression.Member.EmitMemberAssign(generator);

                default:
                    return false;
            }
        }
    }
}