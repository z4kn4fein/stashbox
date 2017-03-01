#if NET45 || NET40
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal class DelegateTarget
    {
    }

    internal class DelegateTarget<T1>
    {
        public T1 V1;

        public DelegateTarget(T1 t1)
        {
            V1 = t1;
        }
    }

    internal class DelegateTarget<T1, T2>
    {
        public T1 V1;
        public T2 V2;

        public DelegateTarget(T1 t1, T2 t2)
        {
            V1 = t1;
            V2 = t2;
        }
    }

    internal class DelegateTarget<T1, T2, T3>
    {
        public T1 V1;
        public T2 V2;
        public T3 V3;

        public DelegateTarget(T1 t1, T2 t2, T3 t3)
        {
            V1 = t1;
            V2 = t2;
            V3 = t3;
        }
    }

    internal class DelegateTarget<T1, T2, T3, T4>
    {
        public T1 V1;
        public T2 V2;
        public T3 V3;
        public T4 V4;

        public DelegateTarget(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            V1 = t1;
            V2 = t2;
            V3 = t3;
            V4 = t4;
        }
    }

    internal class DelegateTarget<T1, T2, T3, T4, T5>
    {
        public T1 V1;
        public T2 V2;
        public T3 V3;
        public T4 V4;
        public T5 V5;

        public DelegateTarget(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            V1 = t1;
            V2 = t2;
            V3 = t3;
            V4 = t4;
            V5 = t5;
        }
    }

    internal class DelegateTarget<T1, T2, T3, T4, T5, T6>
    {
        public T1 V1;
        public T2 V2;
        public T3 V3;
        public T4 V4;
        public T5 V5;
        public T6 V6;

        public DelegateTarget(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            V1 = t1;
            V2 = t2;
            V3 = t3;
            V4 = t4;
            V5 = t5;
            V6 = t6;
        }
    }

    internal class DelegateTarget<T1, T2, T3, T4, T5, T6, T7>
    {
        public T1 V1;
        public T2 V2;
        public T3 V3;
        public T4 V4;
        public T5 V5;
        public T6 V6;
        public T7 V7;

        public DelegateTarget(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            V1 = t1;
            V2 = t2;
            V3 = t3;
            V4 = t4;
            V5 = t5;
            V6 = t6;
            V7 = t7;
        }
    }

    internal class DelegateTarget<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public T1 V1;
        public T2 V2;
        public T3 V3;
        public T4 V4;
        public T5 V5;
        public T6 V6;
        public T7 V7;
        public T8 V8;

        public DelegateTarget(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            V1 = t1;
            V2 = t2;
            V3 = t3;
            V4 = t4;
            V5 = t5;
            V6 = t6;
            V7 = t7;
            V8 = t8;
        }
    }

    internal class DelegateTargetInformation
    {
        public readonly object NestedTarget;
        public readonly FieldInfo[] ConstantFields;
        public readonly object[] Constants;

        public DelegateTargetInformation(object target, object[] constants, FieldInfo[] constantFields)
        {
            this.NestedTarget = target;
            this.Constants = constants;
            this.ConstantFields = constantFields;
        }
    }

    internal static class DelegateTargetSelector
    {
        public static DelegateTargetInformation GetDelegateTarget(Expression expression, List<object> constants)
        {
            Type targetType = null;
            var consts = constants.ToArray();
            var count = consts.Length;

            switch (count)
            {
                case 0:
                    targetType = typeof(DelegateTarget);
                    break;
                case 1:
                    targetType = typeof(DelegateTarget<>);
                    break;
                case 2:
                    targetType = typeof(DelegateTarget<,>);
                    break;
                case 3:
                    targetType = typeof(DelegateTarget<,,>);
                    break;
                case 4:
                    targetType = typeof(DelegateTarget<,,,>);
                    break;
                case 5:
                    targetType = typeof(DelegateTarget<,,,,>);
                    break;
                case 6:
                    targetType = typeof(DelegateTarget<,,,,,>);
                    break;
                case 7:
                    targetType = typeof(DelegateTarget<,,,,,,>);
                    break;
                case 8:
                    targetType = typeof(DelegateTarget<,,,,,,,>);
                    break;
                default:
                    return new DelegateTargetInformation(null, consts, null);
            }

            targetType = count > 0 ? targetType.MakeGenericType(constants.Select(c => c.GetType()).ToArray()) : targetType;
            var target = Activator.CreateInstance(targetType, consts);

            return new DelegateTargetInformation(target, consts, targetType.GetTypeInfo().DeclaredFields.ToArray());
        }

        public static bool TryCollectConstants(Expression expression, List<object> constants)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    return TryCollectConstantsFromConstant((ConstantExpression)expression, constants);
                case ExpressionType.New:
                    return TryCollectConstantsFromArguments(((NewExpression)expression).Arguments, constants);
                case ExpressionType.MemberInit:
                    return TryCollectConstantsFromMemberInit(expression, constants);
                case ExpressionType.Call:
                    var call = (MethodCallExpression)expression;
                    return TryCollectConstants(call.Object, constants) &&
                           TryCollectConstantsFromArguments(call.Arguments, constants);
                case ExpressionType.NewArrayInit:
                    return TryCollectConstantsFromArguments(((NewArrayExpression)expression).Expressions, constants);
                default:
                    if (expression is UnaryExpression unaryExpression)
                        return TryCollectConstants(unaryExpression.Operand, constants);
                    break;
            }

            return false;
        }

        private static bool TryCollectConstantsFromMemberInit(Expression expression, List<object> constants)
        {
            var memberInit = (MemberInitExpression)expression;
            if (!TryCollectConstants(memberInit.NewExpression, constants))
                return false;

            foreach (var binding in memberInit.Bindings)
                if (binding.BindingType == MemberBindingType.Assignment && !TryCollectConstants(((MemberAssignment)binding).Expression, constants))
                    return false;

            return true;
        }

        private static bool TryCollectConstantsFromArguments(ReadOnlyCollection<Expression> arguments, List<object> constants)
        {
            foreach (var expression in arguments)
                if (!TryCollectConstants(expression, constants))
                    return false;

            return true;
        }

        private static bool TryCollectConstantsFromConstant(ConstantExpression expression, List<object> constants)
        {
            if (expression.Value != null)
            {
                var type = expression.Value.GetType();
                if (type != typeof(int) && 
                    type != typeof(double) && 
                    type != typeof(bool) && 
                    type != typeof(string) && 
                    type != typeof(Type) && 
                    !type.IsEnum && 
                    expression.Value != null)
                        constants.Add(expression.Value);
            }

            return true;
        }
    }
}
#endif