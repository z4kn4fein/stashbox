#if NET45 || NET40 || NETSTANDARD1_3
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this LambdaExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            var length = parameters.Length;
            for (var i = 0; i < length; i++)
            {
                var capturedParamIndex = context.ClosureExpressions.GetIndex(parameters[i]);
                if (capturedParamIndex == -1) continue;

                generator.Emit(OpCodes.Ldarg_0);
                generator.LoadParameter(i + 1);
                generator.Emit(OpCodes.Stfld, context.Target.Fields[capturedParamIndex]);
            }

            var lambdaClosureIndex = context.ClosureExpressions.GetIndex(expression);
            if (lambdaClosureIndex == -1) return false;

            var nestedParameters = expression.Parameters.CastToArray();
            var nestedMethod = CreateDynamicMethod(context, expression.ReturnType, expression.Parameters.CastToArray());
            var nestedGenerator = nestedMethod.GetILGenerator();

            if (!expression.Body.TryEmit(nestedGenerator, context, nestedParameters))
                return false;

            nestedGenerator.Emit(OpCodes.Ret);

            var lambda = context.HasClosure
                ? nestedMethod.CreateDelegate(expression.Type, context.Target.Target)
                : nestedMethod.CreateDelegate(expression.Type);

            context.Target.Fields[lambdaClosureIndex].SetValue(context.Target.Target, lambda);

            generator.LoadConstantField(context, lambdaClosureIndex);

            return true;
        }
    }
}
#endif
