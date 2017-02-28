#if NET45 || NET40
using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal static class ExpressionEmitter
    {
        public static bool TryEmit<TValue>(this Expression expression, out TValue resultDelegate) where TValue : class
        {
            resultDelegate = null;
            var constants = new List<object>();

            if (!DelegateTargetSelector.TryCollectConstants(expression, constants))
                return false;

            var targetInfo = DelegateTargetSelector.GetDelegateTarget(expression, constants);
            if (targetInfo == null)
                return false;

            var returnType = targetInfo.Target.GetType();
            var method = new DynamicMethod(string.Empty, typeof(object), new Type[] { returnType }, returnType, true);
            var generator = method.GetILGenerator();

            if (!expression.TryEmit(targetInfo, generator))
                return false;

            generator.Emit(OpCodes.Ret);
            resultDelegate = method.CreateDelegate(typeof(TValue), targetInfo.Target) as TValue;
            return true;
        }

        private static bool TryEmit(this Expression expression, DelegateTargetInformation target, ILGenerator generator)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Call:
                    return ((MethodCallExpression)expression).TryEmit(target, generator);
                case ExpressionType.Constant:
                    return ((ConstantExpression)expression).TryEmit(target, generator);
                case ExpressionType.MemberInit:
                    return ((MemberInitExpression)expression).TryEmit(target, generator);
                case ExpressionType.New:
                    return ((NewExpression)expression).TryEmit(target, generator);
                case ExpressionType.NewArrayInit:
                    return ((NewArrayExpression)expression).TryEmit(target, generator);
            }

            return false;
        }

        private static bool TryEmit(this NewArrayExpression expression, DelegateTargetInformation target, ILGenerator generator)
        {
            var type = expression.Type;
            var itemType = type.GetEnumerableType();

            var instance = generator.DeclareLocal(itemType);

            generator.EmitInteger(expression.Expressions.Count);
            generator.Emit(OpCodes.Newarr, itemType);
            generator.Emit(OpCodes.Stloc, instance);

            var index = 0;

            foreach (var itemExpression in expression.Expressions)
            {
                generator.Emit(OpCodes.Ldloc, instance);
                generator.EmitInteger(index);

                index++;

                if (type.IsValueType)
                    generator.Emit(OpCodes.Ldelema, itemType);

                if (!itemExpression.TryEmit(target, generator))
                    return false;

                if (type.IsValueType)
                    generator.Emit(OpCodes.Stobj, itemType);
                else
                    generator.Emit(OpCodes.Stelem_Ref);
            }

            generator.Emit(OpCodes.Ldloc, instance);

            return true;
        }

        private static bool TryEmit(this MethodCallExpression expression, DelegateTargetInformation target, ILGenerator generator)
        {
            if (!expression.Object.TryEmit(target, generator))
                return false;

            if (expression.Arguments.Any(argument => !argument.TryEmit(target, generator)))
                return false;

            generator.Emit(expression.Method.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, expression.Method);

            return true;
        }

        private static bool TryEmit(this NewExpression expression, DelegateTargetInformation target, ILGenerator generator)
        {
            if (expression.Arguments.Any(argument => !argument.TryEmit(target, generator)))
                return false;

            generator.Emit(OpCodes.Newobj, expression.Constructor);

            return true;
        }

        private static bool TryEmit(this UnaryExpression expression, DelegateTargetInformation target, ILGenerator generator)
        {
            if (!expression.Operand.TryEmit(target, generator))
                return false;

            var type = expression.Type;
            if (type == typeof(object))
                return false;
            generator.Emit(OpCodes.Castclass, type);

            return true;
        }

        private static bool TryEmit(this MemberInitExpression expression, DelegateTargetInformation target, ILGenerator generator)
        {
            if (!expression.NewExpression.TryEmit(target, generator))
                return false;

            var obj = generator.DeclareLocal(expression.Type);
            generator.Emit(OpCodes.Stloc, obj);

            var bindings = expression.Bindings;
            for (int i = 0, n = bindings.Count; i < n; i++)
            {
                var binding = bindings[i];
                if (binding.BindingType != MemberBindingType.Assignment)
                    return false;

                generator.Emit(OpCodes.Ldloc, obj);

                if (!((MemberAssignment)binding).Expression.TryEmit(target, generator))
                    return false;

                if (binding.Member is PropertyInfo property)
                {
                    var setMethod = property.GetSetMethod();
                    if (setMethod == null)
                        return false;
                    generator.Emit(setMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, setMethod);
                }
                else if (binding.Member is FieldInfo field)
                {
                    generator.Emit(OpCodes.Stfld, field);
                }
            }

            generator.Emit(OpCodes.Ldloc, obj);
            return true;
        }

        private static bool TryEmit(this ConstantExpression expression, DelegateTargetInformation target, ILGenerator generator)
        {
            if (expression.Value == null)
            {
                generator.Emit(OpCodes.Ldnull);
                return true;
            }

            var type = expression.Type;

            if (type == typeof(int))
                generator.EmitInteger((int)expression.Value);
            else if (type == typeof(string))
                generator.Emit(OpCodes.Ldstr, (string)expression.Value);
            else if (type == typeof(bool))
                generator.Emit((bool)expression.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            else if (type == typeof(double))
                generator.Emit(OpCodes.Ldc_R8, (double)expression.Value);
            else if (target.ConstantFields == null)
            {
                var constantIndex = Array.IndexOf(target.Constants, expression.Value);

                if (constantIndex < 0)
                    return false;

                var field = target.Target.GetType().GetField("Constants");
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, field);
                generator.EmitInteger(constantIndex);
                generator.Emit(OpCodes.Ldelem_Ref);

                if (expression.Type != typeof(object))
                    generator.Emit(OpCodes.Castclass, expression.Type);

                return true;
            }
            else
            {
                var constantIndex = Array.IndexOf(target.Constants, expression.Value);

                if (constantIndex < 0)
                    return false;

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, target.ConstantFields[constantIndex]);

                return true;
            }

            return true;
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
    }
}
#endif