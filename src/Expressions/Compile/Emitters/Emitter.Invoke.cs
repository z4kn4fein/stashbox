﻿#if IL_EMIT
using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Stashbox.Expressions.Compile.Extensions;

namespace Stashbox.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this InvocationExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            if (!expression.Expression.TryEmit(generator, context, parameters) || !expression.Arguments.TryEmit(generator, context, parameters))
                return false;

            var invokeMethod = expression.Expression.Type.GetSingleMethod("Invoke");
            generator.EmitMethod(invokeMethod);

            return true;
        }
    }
}
#endif
