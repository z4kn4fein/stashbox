#if NET45 || NET40
using Stashbox.BuildUp.Expressions.Compile;
#endif

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
            {
#if NET45 || NET40
                //if (!expression.TryEmit(out factory))
                    factory = Expression.Lambda<Func<object>>(expression).Compile();
#else
                factory = Expression.Lambda<Func<object>>(expression).Compile();
#endif
            }
            return factory;
        }
    }
}
