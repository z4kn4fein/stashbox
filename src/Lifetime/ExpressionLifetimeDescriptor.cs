using Stashbox.Expressions;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a lifetime descriptor which applies to expressions.
    /// </summary>
    public abstract class ExpressionLifetimeDescriptor : LifetimeDescriptor
    {
        private protected override Expression BuildLifetimeAppliedExpression(ExpressionBuilder expressionBuilder, ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type resolveType)
        {
            var expression = GetExpressionForRegistration(expressionBuilder, serviceRegistration, resolutionContext, resolveType);
            return expression == null ? null : this.ApplyLifetime(expression, serviceRegistration, resolutionContext, resolveType);
        }

        /// <summary>
        /// Derived types are using this method to apply their lifetime to the instance creation.
        /// </summary>
        /// <param name="expression">The expression the lifetime should apply to.</param>
        /// <param name="serviceRegistration">The service registration.</param>
        /// <param name="resolutionContext">The info about the actual resolution.</param>
        /// <param name="resolveType">The type of the resolved service.</param>
        /// <returns>The lifetime managed expression.</returns>
        protected abstract Expression ApplyLifetime(Expression expression, ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type resolveType);
    }
}
