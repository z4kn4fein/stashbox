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
                var instance = ((ConstantExpression)expression).Value;
                factory = () => instance;
            }
            else
                factory = Expression.Lambda<Func<object>>(expression).Compile();

            return factory;
        }
    }
}
