using Stashbox.Expressions.Compile.Extensions;
using System;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this ConstantExpression expression, ILGenerator generator, CompilerContext context)
        {
            var value = expression.Value;
            var type = expression.Type;

            if (value == null)
            {
                if (expression.Type.IsValueType)
                    generator.InitValueType(expression.Type);
                else
                    generator.Emit(OpCodes.Ldnull);
                return true;
            }

            if (context.HasClosure && !Utils.IsInPlaceEmittableConstant(type, value))
            {
                var constantIndex = context.Constants.IndexOf(value);
                if (constantIndex == -1) return false;

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, Closure.ConstantsField);
                generator.EmitInteger(constantIndex);
                generator.Emit(OpCodes.Ldelem_Ref);
                if (type.IsValueType)
                    generator.Emit(OpCodes.Unbox_Any, type);
                return true;
            }

            if (generator.TryEmitNumberConstant(type, value))
                return true;

            if (type.IsEnum)
                return generator.TryEmitNumberConstant(Enum.GetUnderlyingType(type), value);

            switch (value)
            {
                case string stringValue:
                    generator.Emit(OpCodes.Ldstr, stringValue);
                    break;
                case Type typeValue:
                    generator.Emit(OpCodes.Ldtoken, typeValue);
                    generator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}