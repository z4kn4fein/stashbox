#if NET45 || NET40 || NETSTANDARD11
using Stashbox.BuildUp.Expressions.Compile;
#endif

namespace System.Linq.Expressions
{
    internal static class ExpressionExtensions
    {
        public static Func<object> CompileDelegate(this Expression expression)
        {
            if (expression.NodeType == ExpressionType.Constant)
            {
                var instance = ((ConstantExpression)expression).Value;
                return () => instance;
            }

#if NET45 || NET40 || NETSTANDARD11
            if (!expression.TryEmit(out Delegate factory))
                factory = Expression.Lambda(expression).Compile();

            return (Func<object>)factory;
#else
            return Expression.Lambda<Func<object>>(expression).Compile();
#endif
        }

        public static Delegate CompileDelegate(this LambdaExpression expression)
        {
#if NET45 || NET40 || NETSTANDARD11
            if (!expression.TryEmit(out Delegate factory))
                factory = expression.Compile();

            return factory;
#else
            return expression.Compile();
#endif
        }
    }
}
