#if NET45 || NET40 || NETSTANDARD1_3
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this MethodCallExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            if (expression.Object != null && !expression.Object.TryEmit(generator, context, parameters))
                return false;

            return expression.Arguments.TryEmit(generator, context, parameters) &&
                generator.EmitMethod(expression.Method);
        }

        private static bool EmitMethod(this ILGenerator generator, MethodInfo info)
        {
            generator.Emit(info.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, info);
            return true;
        }
    }
}
#endif
