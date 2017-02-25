using System.Reflection;

namespace System.Linq.Expressions
{
    internal static class ExpressionExtensions
    {
        public static Func<object> CompileDelegate(this Expression expression)
        {
            Func<object> factory;
            if (expression.NodeType == ExpressionType.Constant)
            {
                var instance = ((ConstantExpression)expression.Optimize()).Value;
                factory = () => instance;
            }
            else
                factory = Expression.Lambda<Func<object>>(expression.Optimize()).Compile();

            return factory;
        }

        public static Expression Optimize(this Expression expression)
        {
            if (expression.NodeType == ExpressionType.Convert)
                expression = ((UnaryExpression)expression).Operand;
            else if (expression.Type.GetTypeInfo().IsValueType)
                expression = Expression.Convert(expression, typeof(object));
            return expression;
        }
    }
}
