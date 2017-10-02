using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a scoped lifetime.
    /// </summary>
    public class ScopedLifetime : LifetimeBase
    {
        private volatile Expression expression;
        private readonly object syncObject = new object();
        private readonly object scopeId = new object();

        /// <inheritdoc />
        public override ILifetime Create() => new ScopedLifetime();

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (this.expression != null) return this.expression;
            lock (this.syncObject)
            {
                if (this.expression != null) return this.expression;
                var expr = base.GetExpression(containerContext, serviceRegistration, objectBuilder, resolutionInfo, resolveType);
                if (expr == null)
                    return null;

                var factory = expr.CompileDelegate(Constants.ScopeExpression);
                this.expression = Expression.Convert(Expression.Call(Constants.ScopeExpression, Constants.GetOrAddScopedItemMethod, Expression.Constant(scopeId), Expression.Constant(factory)), resolveType);
            }

            return this.expression;
        }
    }
}
