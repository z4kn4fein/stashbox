#if NET45 || NET40 || NETSTANDARD1_3
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        public static bool TryEmit(this Expression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Call:
                    return ((MethodCallExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.Invoke:
                    return ((InvocationExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.Parameter:
                    return ((ParameterExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.Lambda:
                    return ((LambdaExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.Convert:
                    return ((UnaryExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.Constant:
                    return ((ConstantExpression)expression).TryEmit(generator, context);

                case ExpressionType.MemberInit:
                    return ((MemberInitExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.New:
                    return ((NewExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.Block:
                    return ((BlockExpression)expression).Expressions.TryEmit(generator, context, parameters);

                case ExpressionType.NewArrayInit:
                    return ((NewArrayExpression)expression).TryEmit(generator, context, parameters);

                case ExpressionType.Default:
                    return ((DefaultExpression)expression).TryEmit(generator);
            }

            return false;
        }
        
        public static DynamicMethod CreateDynamicMethod(CompilerContext context, Type returnType, params ParameterExpression[] parameters)
        {
            if (context.Target == null)
                return new DynamicMethod(string.Empty, returnType, parameters.GetTypes(), typeof(ExpressionEmitter), true);

            var targetType = context.Target.TargetType;
            return new DynamicMethod(string.Empty, returnType, targetType.ConcatDelegateTargetParameter(parameters.GetTypes()), targetType, true);
        }

        public static Type[] GetTypes(this IList<ParameterExpression> parameters)
        {
            var count = parameters.Count;
            if (count == 0)
                return Constants.EmptyTypes;
            if (count == 1)
                return new[] { parameters[0].Type };

            var types = new Type[count];
            for (var i = 0; i < count; i++)
                types[i] = parameters[i].Type;
            return types;
        }

        public static Type[] ConcatDelegateTargetParameter(this Type targetType, Type[] parameters)
        {
            var count = parameters.Length;
            if (count == 0)
                return new[] { targetType };

            var types = new Type[count + 1];
            types[0] = targetType;

            if (count == 1)
                types[1] = parameters[0];
            if (count > 1)
                Array.Copy(parameters, 0, types, 1, count);

            return types;
        }

        private static bool TryEmit(this IList<Expression> expressions, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            for (var i = 0; i < expressions.Count; i++)
                if (!expressions[i].TryEmit(generator, context, parameters))
                    return false;

            return true;
        }

        private static void EmitInteger(this ILGenerator generator, int intValue)
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