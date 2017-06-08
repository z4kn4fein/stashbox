using System;
using System.Linq.Expressions;
using System.Reflection;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;

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

        private static readonly MethodInfo GetScopedValueMethod = typeof(ScopedLifetime).GetSingleMethod(nameof(GetScopedValue), true);

        /// <inheritdoc />
        public override ILifetime Create() => new ScopedLifetime();

        /// <inheritdoc />
        public override Expression GetExpression(IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo,
            Type resolveType)
        {
            if (this.expression != null) return this.expression;
            lock (this.syncObject)
            {
                if (this.expression != null) return this.expression;
                var expr = base.GetExpression(serviceRegistration, objectBuilder, resolutionInfo, resolveType);
                if (expr == null)
                    return null;

                var factory = expr.CompileDelegate(Constants.ScopeExpression);

                var method = GetScopedValueMethod.MakeGenericMethod(resolveType);

                this.expression = Expression.Call(method,
                    Constants.ScopeExpression,
                    Expression.Constant(factory), Expression.Constant(this.scopeId));
            }

            return this.expression;
        }

        private static TValue GetScopedValue<TValue>(IResolutionScope scope, Func<IResolutionScope, object> factory, object scopeId)
        {
            var value = scope.GetScopedItemOrDefault(scopeId);
            if (value == null)
            {
                value = factory(scope);
                scope.AddScopedItem(scopeId, value);
            }

            return (TValue)value;
        }
    }
}
