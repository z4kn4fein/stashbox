#if NET45 || NET40 || IL_EMIT
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this UnaryExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            var typeFrom = expression.Operand.Type;
            var typeTo = expression.Type;

            if (!expression.Operand.TryEmit(generator, context, parameters))
                return false;

            if (typeFrom == typeTo)
                return true;

            if (!typeFrom.GetTypeInfo().IsValueType && typeTo.GetTypeInfo().IsValueType)
                generator.Emit(OpCodes.Unbox_Any, typeTo);
            else if (typeFrom.GetTypeInfo().IsValueType && typeTo == typeof(object))
                generator.Emit(OpCodes.Box, typeFrom);
            else
                generator.Emit(OpCodes.Castclass, typeTo);

            return true;
        }
    }
}
#endif
