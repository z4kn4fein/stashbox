using Stashbox.Infrastructure;
using Stashbox.Resolution;

#if NET45 || NET40 || NETSTANDARD1_3
using Stashbox.BuildUp.Expressions.Compile;
#endif

namespace System.Linq.Expressions
{
    /// <summary>
    /// Holds the <see cref="Expression"/> extension methods.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Compiles an <see cref="Expression"/> to a <see cref="Func{T,R}"/> of <see cref="IResolutionScope"/>, <see cref="object"/>.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <returns>The compiled delegate.</returns>
        public static Func<IResolutionScope, object> CompileDelegate(this Expression expression, ResolutionContext resolutionContext)
        {
            if (expression.NodeType == ExpressionType.Constant)
            {
                var instance = ((ConstantExpression)expression).Value;
                return scope => instance;
            }

            if (resolutionContext.GlobalParameters.Length > 0)
            {
                var originalExpression = expression;
                expression = Expression.Block(resolutionContext.GlobalParameters,
                    resolutionContext.SingleInstructions.Add(originalExpression));
            }

#if NET45 || NET40 || NETSTANDARD1_3
            if (!expression.TryEmit(out Delegate factory, typeof(Func<IResolutionScope, object>), typeof(object),
                resolutionContext.CurrentScopeParameter)) ;
            //factory = Expression.Lambda(expression, resolutionContext.CurrentScopeParameter).Compile();

            return (Func<IResolutionScope, object>)factory;
#else
            return Expression.Lambda<Func<IResolutionScope, object>>(expression, resolutionContext.CurrentScopeParameter).Compile();
#endif
        }

        /// <summary>
        /// Compiles an <see cref="Expression"/> to a <see cref="Func{T,R}"/> of <see cref="IResolutionScope"/>, <see cref="Delegate"/>.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <returns>The compiled delegate.</returns>
        public static Func<IResolutionScope, Delegate> CompileDynamicDelegate(this Expression expression, ResolutionContext resolutionContext)
        {
            if (resolutionContext.GlobalParameters.Length > 0)
            {
                var originalExpression = expression;
                expression = Expression.Block(resolutionContext.GlobalParameters,
                    resolutionContext.SingleInstructions.Add(originalExpression));
            }

#if NET45 || NET40 || NETSTANDARD1_3
            if (!expression.TryEmit(out Delegate factory, typeof(Func<IResolutionScope, Delegate>), typeof(Delegate),
                resolutionContext.CurrentScopeParameter)) ;
            //factory = Expression.Lambda<Func<IResolutionScope, Delegate>>(expression, resolutionContext.CurrentScopeParameter).Compile();

            return (Func<IResolutionScope, Delegate>)factory;
#else
            return Expression.Lambda<Func<IResolutionScope, Delegate>>(expression, resolutionContext.CurrentScopeParameter).Compile();
#endif
        }
    }
}
