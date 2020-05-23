#if IL_EMIT
using Stashbox.BuildUp.Expressions.Compile;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace System.Reflection.Emit
{
    internal static class ILGeneratorExtensions
    {
        public static LocalBuilder PrepareCapturedArgumentsHolderVariable(this ILGenerator generator, int capturedArgumentsCount)
        {
            var local = generator.DeclareLocal(typeof(object[]));
            generator.EmitInteger(capturedArgumentsCount);
            generator.Emit(OpCodes.Newarr, typeof(object));
            generator.Emit(OpCodes.Stloc, local);

            return local;
        }

        public static void CopyParametersToCapturedArgumentsIfAny(this ILGenerator generator, CompilerContext context, ParameterExpression[] parameters)
        {
            var length = context.CapturedArguments.Length;
            for (var i = 0; i < length; i++)
            {
                var arg = context.CapturedArguments[i];
                var paramIndex = parameters.GetReferenceIndex(arg);
                if (paramIndex == -1) continue;

                generator.LoadCapturedArgumentHolder(context);
                generator.EmitInteger(i);
                generator.LoadParameter(paramIndex + (context.IsNestedLambda ? 2 : 1));
                if (arg.Type.GetTypeInfo().IsValueType)
                    generator.Emit(OpCodes.Box, arg.Type);
                generator.Emit(OpCodes.Stelem_Ref);
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
                case 0: generator.Emit(OpCodes.Ldc_I4_0); break;
                case 1: generator.Emit(OpCodes.Ldc_I4_1); break;
                case 2: generator.Emit(OpCodes.Ldc_I4_2); break;
                case 3: generator.Emit(OpCodes.Ldc_I4_3); break;
                case 4: generator.Emit(OpCodes.Ldc_I4_4); break;
                case 5: generator.Emit(OpCodes.Ldc_I4_5); break;
                case 6: generator.Emit(OpCodes.Ldc_I4_6); break;
                case 7: generator.Emit(OpCodes.Ldc_I4_7); break;
                case 8: generator.Emit(OpCodes.Ldc_I4_8); break;
                default:
                    if (intValue >= sbyte.MinValue && intValue <= sbyte.MaxValue)
                        generator.Emit(OpCodes.Ldc_I4_S, (sbyte)intValue);
                    else
                        generator.Emit(OpCodes.Ldc_I4, intValue);
                    break;
            }
        }

        public static void LoadParameter(this ILGenerator generator, int index)
        {
            switch (index)
            {
                case 0: generator.Emit(OpCodes.Ldarg_0); break;
                case 1: generator.Emit(OpCodes.Ldarg_1); break;
                case 2: generator.Emit(OpCodes.Ldarg_2); break;
                case 3: generator.Emit(OpCodes.Ldarg_3); break;
                default:
                    if (index <= byte.MaxValue)
                        generator.Emit(OpCodes.Ldarg_S, (byte)index);
                    else
                        generator.Emit(OpCodes.Ldarg, index);
                    break;
            }
        }

        public static void InitValueType(this ILGenerator generator, Type type)
        {
            var lb = generator.DeclareLocal(type);
            generator.Emit(OpCodes.Ldloca, lb);
            generator.Emit(OpCodes.Initobj, type);
            generator.Emit(OpCodes.Ldloc, lb);
        }
    }
}
#endif
