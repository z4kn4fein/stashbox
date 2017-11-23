#if NET45 || NET40 || IL_EMIT
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this NewArrayExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            var type = expression.Type;
            var typeInfo = type.GetTypeInfo();
            var itemType = type.GetEnumerableType();

            var length = expression.Expressions.Count;

            generator.EmitInteger(length);
            generator.Emit(OpCodes.Newarr, itemType);

            for (var i = 0; i < length; i++)
            {
                generator.Emit(OpCodes.Dup);
                generator.EmitInteger(i);

                if (!expression.Expressions[i].TryEmit(generator, context, parameters))
                    return false;

                if (typeInfo.IsValueType)
                    generator.Emit(OpCodes.Stelem, itemType);
                else
                    generator.Emit(OpCodes.Stelem_Ref);
            }

            return true;
        }
    }
}
#endif
