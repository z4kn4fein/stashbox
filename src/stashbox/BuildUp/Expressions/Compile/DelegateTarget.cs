#if NET45 || NET40
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Stashbox.Utils;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal class Constants
    {
        public List<object> ConstantObjects { get; }
        public List<Expression> ConstantExpressions { get; }
        public List<LambdaTargetInformation> Lambdas { get; }
        public List<ParameterExpression> Parameters { get; }
        public List<ParameterExpression> Locals { get; }

        public Constants()
        {
            this.ConstantObjects = new List<object>();
            this.ConstantExpressions = new List<Expression>();
            this.Lambdas = new List<LambdaTargetInformation>();
            this.Parameters = new List<ParameterExpression>();
            this.Locals = new List<ParameterExpression>();
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
        public readonly List<Expression> Constants;
        public readonly List<LambdaTargetInformation> Lambdas;
        public readonly List<ParameterExpression> Parameters;
        public readonly List<LocalBuilder> LocalBuilders;
        public readonly List<ParameterExpression> Locals;

        public DelegateTargetInformation(object target, FieldInfo[] constantFields, List<Expression> constants,
            List<LambdaTargetInformation> lambdas, List<ParameterExpression> parameters, List<ParameterExpression> locals)
        {
            this.Locals = locals;
            this.Target = target;
            this.ConstantFields = constantFields;
            this.Constants = constants;
            this.Lambdas = lambdas;
            this.Parameters = parameters;
            this.LocalBuilders = new List<LocalBuilder>();
        }
    }

    internal static class DelegateTargetSelector
    {
        private static int TypeCounter;

        private static readonly ConcurrentTree<Type> TargetTypes = new ConcurrentTree<Type>();

        private static Type GetOrAddTargetType(Constants constants)
        {
            var length = constants.ConstantExpressions.Count;
            var types = new Type[length];

            var foundType = TargetTypes.GetOrDefault(length);
            if (foundType != null)
            {
                if (length <= 0) return foundType;

                for (var i = 0; i < length; i++)
                    types[i] = constants.ConstantExpressions[i].Type;

                return foundType.MakeGenericType(types);
            }

            var fields = new FieldInfo[length];
            var typeBuilder = ExpressionEmitter.ModuleBuilder.Value.DefineType("DT" + Interlocked.Increment(ref TypeCounter), TypeAttributes.Public);

            if (length > 0)
            {
                var typeParamNames = new string[length];
                for (var i = 0; i < length; i++)
                    typeParamNames[i] = "T" + i;

                var typeParams = typeBuilder.DefineGenericParameters(typeParamNames);

                for (var i = 0; i < length; i++)
                {
                    types[i] = constants.ConstantExpressions[i].Type;
                    fields[i] = typeBuilder.DefineField("F" + i, typeParams[i], FieldAttributes.Public);
                }


                var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, typeParams);
                var generator = constructor.GetILGenerator();

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));

                for (var i = 0; i < length; i++)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.LoadParameter(i + 1);
                    generator.Emit(OpCodes.Stfld, fields[i]);
                }

                generator.Emit(OpCodes.Ret);
            }

            var type = typeBuilder.CreateType();
            TargetTypes.AddOrUpdate(length, type);
            return length > 0 ? type.MakeGenericType(types) : type;
        }

        public static DelegateTargetInformation GetDelegateTarget(Constants constants)
        {
            var type = GetOrAddTargetType(constants);
            var target = Activator.CreateInstance(type, constants.ConstantObjects.ToArray());
            return new DelegateTargetInformation(target, type.GetTypeInfo().DeclaredFields as FieldInfo[], constants.ConstantExpressions,
                constants.Lambdas, constants.Parameters, constants.Locals);
        }

        public static bool TryCollectConstants(this Expression expression, Constants constants, params ParameterExpression[] parameters)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:
                    return ((ParameterExpression)expression).TryCollectConstants(constants, parameters);
                case ExpressionType.Lambda:
                    return ((LambdaExpression)expression).TryCollectConstants(constants, parameters);
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expression).TryCollectConstants(constants, parameters);
                case ExpressionType.Constant:
                    return ((ConstantExpression)expression).TryCollectConstants(constants);
                case ExpressionType.New:
                    return ((NewExpression)expression).Arguments.TryCollectConstants(constants, parameters);
                case ExpressionType.MemberInit:
                    return ((MemberInitExpression)expression).TryCollectConstants(constants, parameters);
                case ExpressionType.Block:
                    return ((BlockExpression)expression).TryCollectConstants(constants, parameters);
                case ExpressionType.Call:
                    var call = (MethodCallExpression)expression;
                    return (call.Object == null || call.Object.TryCollectConstants(constants, parameters)) &&
                           call.Arguments.TryCollectConstants(constants, parameters);
                case ExpressionType.Invoke:
                    var invoke = (InvocationExpression)expression;
                    return invoke.Expression.TryCollectConstants(constants, parameters) &&
                           invoke.Arguments.TryCollectConstants(constants, parameters);
                case ExpressionType.NewArrayInit:
                    return ((NewArrayExpression)expression).Expressions.TryCollectConstants(constants, parameters);
                default:
                    if (expression is UnaryExpression unaryExpression)
                        return unaryExpression.Operand.TryCollectConstants(constants, parameters);

                    if (expression is BinaryExpression binaryExpression)
                        return binaryExpression.Left.TryCollectConstants(constants, parameters) &&
                            binaryExpression.Right.TryCollectConstants(constants, parameters);

                    break;
            }

            return false;
        }

        private static bool TryCollectConstants(this MemberInitExpression expression, Constants constants, params ParameterExpression[] parameters)
        {
            if (!expression.NewExpression.TryCollectConstants(constants, parameters))
                return false;

            return expression.Bindings.All(binding => binding.BindingType != MemberBindingType.Assignment ||
                ((MemberAssignment)binding).Expression.TryCollectConstants(constants, parameters));
        }

        private static bool TryCollectConstants(this MemberExpression expression, Constants constants, params ParameterExpression[] parameters) =>
            expression.Expression.TryCollectConstants(constants, parameters);

        private static bool TryCollectConstants(this IEnumerable<Expression> expressions, Constants constants, params ParameterExpression[] parameters) =>
            expressions.All(expression => expression.TryCollectConstants(constants, parameters));

        private static bool TryCollectConstants(this BlockExpression expression, Constants constants, params ParameterExpression[] parameters)
        {
            constants.Locals.AddRange(expression.Variables);

            return expression.Expressions.TryCollectConstants(constants, parameters);
        }

        private static bool TryCollectConstants(this ParameterExpression expression, Constants constants, params ParameterExpression[] parameters)
        {
            if (Array.IndexOf(parameters, expression) != -1) return true;
            if (constants.Parameters.Contains(expression) || constants.Locals.Contains(expression)) return true;

            constants.Parameters.Add(expression);
            constants.ConstantExpressions.Add(expression);
            constants.ConstantObjects.Add(null);

            return true;
        }

        private static bool TryCollectConstants(this LambdaExpression expression, Constants constants, params ParameterExpression[] parameters)
        {
            if (!expression.Body.TryEmit(expression.Type, expression.Body.Type, out Delegate lambda, out DelegateTargetInformation lambdaTarget, expression.Parameters.ToArray()))
                return false;

            constants.ConstantObjects.Add(lambda);
            constants.ConstantExpressions.Add(expression);
            constants.Lambdas.Add(new LambdaTargetInformation(expression, lambdaTarget));

            var length = lambdaTarget.Parameters.Count;
            for (var i = 0; i < length; i++)
            {
                var currentNotFoundParam = lambdaTarget.Parameters[i];
                if (Array.IndexOf(parameters, currentNotFoundParam) != -1) continue;
                constants.ConstantExpressions.Add(currentNotFoundParam);
                constants.ConstantObjects.Add(null);
            }

            return true;
        }

        private static bool TryCollectConstants(this ConstantExpression expression, Constants constants)
        {
            if (expression.Value == null) return true;
            var type = expression.Value.GetType();
            if (type == typeof(int) || type == typeof(double) || type == typeof(bool) || type == typeof(string) ||
                type == typeof(Type) || type.IsEnum || expression.Value == null) return true;
            constants.ConstantExpressions.Add(expression);
            constants.ConstantObjects.Add(expression.Value);

            return true;
        }
    }
}
#endif