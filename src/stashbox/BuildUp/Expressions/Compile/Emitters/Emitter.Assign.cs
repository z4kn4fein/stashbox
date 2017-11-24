#if NET45 || NET40 || IL_EMIT
using System.Collections.Generic;
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

                    var localIndex = context.DefinedVariables.GetIndex(expression.Left);
                    if (localIndex == -1) return false;

                    if (!expression.Right.TryEmit(generator, context, parameters))
                        return false;
                        
                    generator.Emit(OpCodes.Stloc, context.LocalBuilders[localIndex]);

                    if (!context.HasCapturedVariablesArgument) return true;

                    var paramIndex = context.CapturedArguments.GetIndex(expression.Left);
                    if (paramIndex == -1) return true;

                    generator.LoadCapturedArgumentHolder(context);
                    generator.Emit(OpCodes.Ldloc, context.LocalBuilders[localIndex]);
                    generator.Emit(OpCodes.Stfld, context.CapturedArgumentsHolder.Fields[paramIndex]);

                    return true;

                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression.Left;

                    if (memberExpression.Expression != null && !memberExpression.Expression.TryEmit(generator, context, parameters))
                        return false;

                    if (!expression.Right.TryEmit(generator, context, parameters))
                        return false;

                    return memberExpression.Member.EmitMemberAssign(generator);

                default:
                    return false;
            }
        }
    }
}
#endif