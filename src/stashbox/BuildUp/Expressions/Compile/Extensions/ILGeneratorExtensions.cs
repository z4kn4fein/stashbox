#if NET45 || NET40 || IL_EMIT
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Stashbox.BuildUp.Expressions.Compile;

namespace System.Reflection.Emit
{
    internal static class ILGeneratorExtensions
    {

        public static LocalBuilder PrepareCapturedArgumentsHolderVariable(this ILGenerator generator, Type capturedArgumentType)
        {
            var local = generator.DeclareLocal(capturedArgumentType);
            generator.Emit(OpCodes.Newobj, capturedArgumentType.GetTypeInfo().DeclaredConstructors.First());
            generator.Emit(OpCodes.Stloc, local);

            return local;
        }

        public static void CopyParametersToCapturedArgumentsIfAny(this ILGenerator generator, CompilerContext context, ParameterExpression[] parameters)
        {
            var length = context.CapturedArguments.Length;
            for (var i = 0; i < length; i++)
            {
                var arg = context.CapturedArguments[i];
                var paramIndex = parameters.GetIndex(arg);
                if (paramIndex != -1)
                {
                    generator.LoadCapturedArgumentHolder(context);
                    generator.LoadParameter(paramIndex + (context.IsNestedLambda ? 2 : 1));
                    generator.Emit(OpCodes.Stfld,
                        context.CapturedArgumentsHolder.Fields[i]);
                }
            }
        }

        public static bool EmitMethod(this ILGenerator generator, MethodInfo info)
        {
            generator.Emit(info.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, info);
            return true;
        }

        public static void LoadCapturedArgumentHolder(this ILGenerator generator, CompilerContext context)
        {
            if (!context.IsNestedLambda)
                generator.Emit(OpCodes.Ldloc, context.CapturedArgumentsHolderVariable);
            else
                generator.Emit(context.HasClosure ? OpCodes.Ldarg_1 : OpCodes.Ldarg_0);
        }

        public static void EmitInteger(this ILGenerator generator, int intValue)
        {
            switch (intValue)
            {
                case 0:
                    generator.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    generator.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    generator.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    generator.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    generator.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    generator.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    generator.Emit(OpCodes.Ldc_I4, intValue);
                    break;
            }
        }

        public static void LoadParameter(this ILGenerator generator, int index)
        {
            switch (index)
            {
                case 0:
                    generator.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (index <= byte.MaxValue)
                        generator.Emit(OpCodes.Ldarg_S, (byte)index);
                    else
                        generator.Emit(OpCodes.Ldarg, index);
                    break;
            }
        }
    }
}
#endif
