using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Stashbox.Expressions.Compile.Emitters
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

            var typeToIsNullable = typeTo.IsNullableType();
            var typeToUnderlyingType = Nullable.GetUnderlyingType(typeTo);

            ConstructorInfo? constructor;
            if (typeToIsNullable && typeFrom == typeToUnderlyingType && (constructor = typeTo.GetFirstConstructor()) != null)
            {
                generator.Emit(OpCodes.Newobj, constructor);
                return true;
            }

            if (!typeFrom.IsValueType && typeTo.IsValueType)
                generator.Emit(OpCodes.Unbox_Any, typeTo);
            else if (typeFrom.IsValueType && typeTo == typeof(object))
                generator.Emit(OpCodes.Box, typeFrom);
            else
                generator.Emit(OpCodes.Castclass, typeTo);

            return true;
        }
    }
}
