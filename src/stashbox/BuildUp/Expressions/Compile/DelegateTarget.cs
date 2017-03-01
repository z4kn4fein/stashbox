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

    internal class Constants
    {
        public List<object> ConstantObjects { get; private set; }
        public List<Expression> ConstantExpressions { get; private set; }
        public List<LambdaTargetInformation> Lambdas { get; private set; }

        public Constants()
        {
            this.ConstantObjects = new List<object>();
            this.ConstantExpressions = new List<Expression>();
            this.Lambdas = new List<LambdaTargetInformation>();
        }
    }

    internal class LambdaTargetInformation
    {
        public LambdaExpression LambdaExpression { get; private set; }
        public DelegateTargetInformation DelegateTargetInfo { get; private set; }

        public LambdaTargetInformation(LambdaExpression lambdaExpression, DelegateTargetInformation delegateTargetInfo)
        {
            this.LambdaExpression = lambdaExpression;
            this.DelegateTargetInfo = delegateTargetInfo;
        }
    }

    internal class DelegateTargetInformation
    {
        public readonly object Target;
        public readonly FieldInfo[] ConstantFields;
        public readonly Expression[] Constants;
        public readonly LambdaTargetInformation[] Lambdas;

        public DelegateTargetInformation(object target, FieldInfo[] constantFields, Expression[] constants, LambdaTargetInformation[] lambdas)
        {
            this.Target = target;
            this.ConstantFields = constantFields;
            this.Constants = constants;
            this.Lambdas = lambdas;
        }
    }

    internal class ArrayDelegateTarget
    {
        public readonly object[] Constants;

        public ArrayDelegateTarget(object[] constants)
        {
            this.Constants = constants;
        }
    }

    internal static class DelegateTargetSelector
    {
        public static DelegateTargetInformation GetDelegateTarget(Expression expression, Constants constants)
        {
            Type targetType = null;
            var consts = constants.ConstantObjects.ToArray();
            var constsExprs = constants.ConstantExpressions.ToArray();
            var lambdas = constants.Lambdas.ToArray();
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
                    return new DelegateTargetInformation(new ArrayDelegateTarget(consts), null, constsExprs, lambdas);
            }

            targetType = count > 0 ? targetType.MakeGenericType(constsExprs.Select(c => c.Type).ToArray()) : targetType;
            var target = Activator.CreateInstance(targetType, consts);

            return new DelegateTargetInformation(target, targetType.GetTypeInfo().DeclaredFields.ToArray(), constsExprs, lambdas);
        }

        public static bool TryCollectConstants(this Expression expression, Constants constants, params ParameterExpression[] parameters)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:
                    return ((ParameterExpression)expression).TryCollectConstants(constants, parameters);
                case ExpressionType.Lambda:
                    return ((LambdaExpression)expression).TryCollectConstants(constants, parameters);
                case ExpressionType.Constant:
                    return ((ConstantExpression)expression).TryCollectConstants(constants, parameters);
                case ExpressionType.New:
                    return ((NewExpression)expression).Arguments.TryCollectConstants(constants, parameters);
                case ExpressionType.MemberInit:
                    return ((MemberInitExpression)expression).TryCollectConstants(constants, parameters);
                case ExpressionType.Call:
                    var call = (MethodCallExpression)expression;
                    return call.Object.TryCollectConstants(constants, parameters) &&
                           call.Arguments.TryCollectConstants(constants, parameters);
                case ExpressionType.NewArrayInit:
                    return ((NewArrayExpression)expression).Expressions.TryCollectConstants(constants, parameters);
                default:
                    if (expression is UnaryExpression unaryExpression)
                        return unaryExpression.Operand.TryCollectConstants(constants, parameters);
                    break;
            }

            return false;
        }

        private static bool TryCollectConstants(this MemberInitExpression expression, Constants constants, params ParameterExpression[] parameters)
        {
            if (!expression.NewExpression.TryCollectConstants(constants, parameters))
                return false;

            foreach (var binding in expression.Bindings)
                if (binding.BindingType == MemberBindingType.Assignment && !((MemberAssignment)binding).Expression.TryCollectConstants(constants, parameters))
                    return false;

            return true;
        }

        private static bool TryCollectConstants(this ReadOnlyCollection<Expression> arguments, Constants constants, params ParameterExpression[] parameters)
        {
            foreach (var expression in arguments)
                if (!expression.TryCollectConstants(constants, parameters))
                    return false;

            return true;
        }

        private static bool TryCollectConstants(this ParameterExpression expression, Constants constants, params ParameterExpression[] parameters)
        {
            if(Array.IndexOf(parameters, expression) == -1)
            {
                constants.ConstantExpressions.Add(expression);
                constants.ConstantObjects.Add(null);
            }

            return true;
        }

        private static bool TryCollectConstants(this LambdaExpression expression, Constants constants, params ParameterExpression[] parameters)
        {
            if (!expression.Body.TryEmit(expression.Type, expression.Body.Type, out object lambda, out DelegateTargetInformation lambdaTarget, expression.Parameters.ToArray()))
                return false;

            constants.ConstantObjects.Add(lambda);
            constants.ConstantExpressions.Add(expression);
            constants.Lambdas.Add(new LambdaTargetInformation(expression, lambdaTarget));

            var notFoundParameters = lambdaTarget.Constants.OfType<ParameterExpression>().ToArray();
            var length = notFoundParameters.Length;
            for (int i = 0; i < length; i++)
            {
                var currentNotFoundParam = notFoundParameters[i];
                if(Array.IndexOf(parameters, currentNotFoundParam) == -1)
                {
                    constants.ConstantExpressions.Add(currentNotFoundParam);
                    constants.ConstantObjects.Add(null);
                }
            }

            return true;
        }

        private static bool TryCollectConstants(this ConstantExpression expression, Constants constants, params ParameterExpression[] parameters)
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
                {
                    constants.ConstantExpressions.Add(expression);
                    constants.ConstantObjects.Add(expression.Value);
                }
            }

            return true;
        }
    }
}
#endif