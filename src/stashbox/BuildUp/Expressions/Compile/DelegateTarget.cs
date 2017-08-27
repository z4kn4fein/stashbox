#if NET45 || NET40 || NETSTANDARD1_3 || NETSTANDARD2_0
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
            this.Target = target;
            this.ConstantFields = constantFields;
            this.Constants = constants;
            this.Lambdas = lambdas;
            this.Parameters = parameters;
            this.Locals = locals;
            this.LocalBuilders = new List<LocalBuilder>();
        }
    }

    internal static class DelegateTargetSelector
    {
        private static int typeCounter;

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
            var typeBuilder = ExpressionEmitter.ModuleBuilder.Value.DefineType("DT" + Interlocked.Increment(ref typeCounter), TypeAttributes.Public);

            if (length > 0)
            {
                var typeParamNames = new string[length];
                for (var i = 0; i < length; i++)
                    typeParamNames[i] = "T" + i;

                var typeParams = typeBuilder.DefineGenericParameters(typeParamNames);
#if NETSTANDARD1_3 || NETSTANDARD2_0
                var genericTypes = new Type[length];
#endif

                for (var i = 0; i < length; i++)
                {
                    types[i] = constants.ConstantExpressions[i].Type;
#if NETSTANDARD1_3 || NETSTANDARD2_0
                    var genericType = typeParams[i].AsType();
                    genericTypes[i] = genericType;
                    fields[i] = typeBuilder.DefineField("F" + i, genericType, FieldAttributes.Public);
#else
                    fields[i] = typeBuilder.DefineField("F" + i, typeParams[i], FieldAttributes.Public);
#endif
                }

#if NETSTANDARD1_3 || NETSTANDARD2_0
                var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, genericTypes);
#else
                var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, typeParams);
#endif
                var generator = constructor.GetILGenerator();

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Call, Stashbox.Constants.ObjectConstructor);

                for (var i = 0; i < length; i++)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.LoadParameter(i + 1);
                    generator.Emit(OpCodes.Stfld, fields[i]);
                }

                generator.Emit(OpCodes.Ret);
            }
#if NETSTANDARD1_3 || NETSTANDARD2_0
            var type = typeBuilder.CreateTypeInfo().AsType();
#else
            var type = typeBuilder.CreateType();
#endif
            TargetTypes.AddOrUpdate(length, type);
            return length > 0 ? type.MakeGenericType(types) : type;
        }

        public static DelegateTargetInformation GetDelegateTarget(Constants constants)
        {
            if (constants.ConstantExpressions.Count > 0)
            {
                var type = GetOrAddTargetType(constants);
                var target = Activator.CreateInstance(type, constants.ConstantObjects.CastToArray());
                return new DelegateTargetInformation(target, type.GetTypeInfo().DeclaredFields as FieldInfo[], constants.ConstantExpressions,
                    constants.Lambdas, constants.Parameters, constants.Locals);
            }

            return new DelegateTargetInformation(null, null, null, null, null, constants.Locals);
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

            for (int i = 0; i < expression.Bindings.Count; i++)
            {
                var binding = expression.Bindings[i];
                if (binding.BindingType != MemberBindingType.Assignment || !((MemberAssignment)binding).Expression.TryCollectConstants(constants, parameters))
                    return false;
            }

            return true;
        }

        private static bool TryCollectConstants(this MemberExpression expression, Constants constants, params ParameterExpression[] parameters) =>
            expression.Expression.TryCollectConstants(constants, parameters);

        private static bool TryCollectConstants(this IList<Expression> expressions, Constants constants, params ParameterExpression[] parameters)
        {
            for (var i = 0; i < expressions.Count; i++)
                if (!expressions[i].TryCollectConstants(constants, parameters))
                    return false;

            return true;
        }

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
            if (!expression.Body.TryEmit(expression.Type, expression.Body.Type, out Delegate lambda, out DelegateTargetInformation lambdaTarget, expression.Parameters.CastToArray()))
                return false;

            constants.ConstantObjects.Add(lambda);
            constants.ConstantExpressions.Add(expression);
            constants.Lambdas.Add(new LambdaTargetInformation(expression, lambdaTarget));

            if (lambdaTarget.Parameters == null) return true;

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

            constants.ConstantExpressions.Add(expression);
            constants.ConstantObjects.Add(expression.Value);

            return true;
        }
    }
}
#endif