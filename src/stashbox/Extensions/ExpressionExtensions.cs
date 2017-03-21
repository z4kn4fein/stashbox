using Stashbox.Infrastructure;

#if NET45 || NET40
using Stashbox.BuildUp.Expressions.Compile;
#endif

namespace System.Linq.Expressions
{
    internal static class ExpressionExtensions
    {
        public static Func<IResolutionScope, object> CompileDelegate(this Expression expression, ParameterExpression scopeParameter)
        {
            if (expression.NodeType == ExpressionType.Constant)
            {
                var instance = ((ConstantExpression)expression).Value;
                return scope => instance;
            }

#if NET45 || NET40
            if (!expression.TryEmit(out Delegate factory, typeof(Func<IResolutionScope, object>), typeof(object), scopeParameter))
                factory = Expression.Lambda(expression, scopeParameter).Compile();

            return (Func<IResolutionScope, object>)factory;
#else
            return Expression.Lambda<Func<IResolutionScope, object>>(expression, scopeParameter).Compile();
#endif
        }

        public static Func<IResolutionScope, Delegate> CompileDelegate(this LambdaExpression expression, ParameterExpression scopeParameter)
        {
#if NET45 || NET40
            if (!expression.TryEmit(out Delegate factory, typeof(Func<IResolutionScope, Delegate>), typeof(Delegate), scopeParameter))
                factory = Expression.Lambda<Func<IResolutionScope, Delegate>>(expression, scopeParameter).Compile();

            return (Func<IResolutionScope, Delegate>)factory;
#else
            return Expression.Lambda<Func<IResolutionScope, Delegate>>(expression, scopeParameter).Compile();
#endif
        }
    }
}
