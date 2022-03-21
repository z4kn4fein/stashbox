using Stashbox.Expressions.Compile.Extensions;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this InvocationExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            if (!expression.Expression.TryEmit(generator, context, parameters) || !expression.Arguments.TryEmit(generator, context, parameters))
                return false;

            var invokeMethod = expression.Expression.Type.GetMethod("Invoke")!;
            generator.EmitMethod(invokeMethod);

            return true;
        }
    }
}
