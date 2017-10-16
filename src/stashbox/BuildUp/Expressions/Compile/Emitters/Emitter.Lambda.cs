#if NET45 || NET40 || NETSTANDARD1_3
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this LambdaExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            var lambdaClosureIndex = context.StoredExpressions.GetIndex(expression);
            if (lambdaClosureIndex == -1) return false;

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, context.Target.Fields[lambdaClosureIndex]);

            if (context.HasCapturedVariablesArgument)
            {
                if (!context.HasCapturedVariablesArgumentConstructed)
                    generator.Emit(OpCodes.Newobj, context.CapturedArgumentsHolder.TargetType.GetTypeInfo().DeclaredConstructors.First());
                else
                    generator.Emit(OpCodes.Ldarg_1);

                var length = context.CapturedArguments.Length;
                for (var i = 0; i < length; i++)
                {
                    var arg = context.CapturedArguments[i];
                    var paramIndex = parameters.GetIndex(arg);
                    if (paramIndex != -1)
                    {
                        generator.Emit(OpCodes.Dup);

                        generator.LoadParameter(paramIndex + 1);
                        generator.Emit(OpCodes.Stfld,
                            context.CapturedArgumentsHolder.Fields[i]);

                        continue;
                    }

                    var definedVariableIndex = context.DefinedVariables.GetIndex(arg);
                    if (definedVariableIndex != -1)
                    {
                        generator.Emit(OpCodes.Dup);

                        generator.Emit(OpCodes.Ldloc, context.LocalBuilders[definedVariableIndex]);
                        generator.Emit(OpCodes.Stfld,
                            context.CapturedArgumentsHolder.Fields[i]);
                    }
                }
                
                generator.EmitMethod(GetCurryClosureMethod(context.ConcatCapturedArgumentWithParameter(parameters.GetTypes())));
                return true;
            }

            return true;
        }
    }
}
#endif
