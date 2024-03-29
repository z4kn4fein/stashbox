﻿using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.Expressions.Compile.Emitters;

internal static partial class Emitter
{
    private static bool TryEmit(this NewExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
    {
        if (!expression.Arguments.TryEmit(generator, context, parameters))
            return false;

        generator.Emit(OpCodes.Newobj, expression.Constructor!);

        return true;
    }
}