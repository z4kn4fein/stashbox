#if NET45 || NET40
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

#if NET45 || NET40
            if (!expression.TryEmit<Func<object>>(out object factory))
                factory = Expression.Lambda<Func<object>>(expression).Compile();

            return (Func<object>)factory;
#else
            return Expression.Lambda<Func<object>>(expression).Compile();
#endif
        }

        public static Delegate CompileDelegate(this LambdaExpression expression)
        {
#if NET45 || NET40
            if (!expression.TryEmit(out object factory))
                factory = expression.Compile();

            return (Delegate)factory;
#else
            return expression.Compile();
#endif
        }
    }
}
